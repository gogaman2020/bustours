using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Selections;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Сервис подбора мест.
    /// </summary>
    [InjectAsSingleton]
    public class SelectionService : ISelectionService
    {
        private readonly ISelectionRepository _selectionRepository;

        private readonly RuleSelector _rsTable1 = new RuleSelector("Rules_1_Table");
        private readonly RuleSelector _rsSeat1  = new RuleSelector("Rules_1_Seat");
        private readonly RuleSelector _rsTable2 = new RuleSelector("Rules_2_Table");
        private readonly RuleSelector _rsSeat2  = new RuleSelector("Rules_2_Seat");
        private readonly RuleSelector _rsTable3 = new RuleSelector("Rules_3_Table");
        private readonly RuleSelector _rsTable4 = new RuleSelector("Rules_4_Table");

        private readonly Dictionary<int, RuleSelectorWrapper> _ruleSelectorWrappersTable =
            new Dictionary<int, RuleSelectorWrapper>();
        private readonly Dictionary<int, RuleSelectorWrapper> _ruleSelectorWrappersSeat =
            new Dictionary<int, RuleSelectorWrapper>();

        private readonly Dictionary<SelectionVariant, ILookup<int, RuleSelectorWrapper>> _ruleSelectors =
            new Dictionary<SelectionVariant, ILookup<int, RuleSelectorWrapper>>();

        /// <summary>
        /// Конструктор класса <see cref="SelectionService"/>
        /// </summary>
        /// <param name="selectionRepository">Репозиторий для подбора мест.</param>
        public SelectionService(
            ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository;

            Init();
        }

        /// <inheritdoc/>
        public async Task<ResponseSelection> SelectAsync(SelectionInfo selectionInfo)
        {
            var tourInfo = await _selectionRepository.GetTourInfoAsync(selectionInfo.TourId);

            var selectionResult = new SelectionResult { AvailableObjects = new List<BusObject>() };

            var tables = new List<ResponseTable>();

            if (selectionInfo.OrderType == OrderType.Regular)
            {
                var busModel = ConvertToBusModel(tourInfo, selectionInfo);

                var autoSelectSuccessed = false;

                if (selectionInfo.ClickedObject == null)
                {
                    autoSelectSuccessed = TryAutoSelect(busModel, selectionInfo);
                }
                else
                {
                    busModel.SelectionSwitch(selectionInfo);
                }

                selectionResult =
                    autoSelectSuccessed
                        ? new SelectionResult { AvailableObjects = new List<BusObject>() }
                        : ApplyBestRule(busModel, selectionInfo);

                tables = tourInfo.Select(p => ConvertToResponseModel(p, busModel)).ToList();

                // Корректировка для места инвалидов
                if (selectionInfo.DisabledGuestCount > 0)
                {
                    tables.Where(x => x.IsSelected).ToList().ForEach(t =>
                    {
                        t.IsSelected = false;
                        t.Seats.ToList().ForEach(x => x.IsSelected = true);
                    });

                    var seatToUnselect = tables.SelectMany(x => x.Seats).Where(x => x.IsSelected).LastOrDefault();
                    if (seatToUnselect != null)
                    {
                        seatToUnselect.IsSelected = false;
                    }

                    var disabledSeat = tables.SelectMany(x => x.Seats).FirstOrDefault(x => x.Type == SeatType.Disabled);

                    var busObject = new BusObject { Id = disabledSeat.Id, Type = BusObjectTypes.Seat };
                    selectionResult.AvailableObjects.Add(busObject);
                    selectionInfo.SelectedObjects.Add(busObject);
                    disabledSeat.IsSelected = true;
                } 
                else
                {
                    var seatToUnselect = tables
                        .SelectMany(x => x.Seats)
                        .Where(x => x.IsSelected && x.Type == SeatType.Disabled)
                        .FirstOrDefault();

                    if (seatToUnselect != null)
                    {
                        seatToUnselect.IsSelected = false;
                        selectionInfo.SelectedObjects.RemoveAll(x => x.Id == seatToUnselect.Id && x.Type == BusObjectTypes.Seat);
                    }

                    var spareTables = tables
                        .Where(t => t.Seats.Any(s => !s.IsSelected && !s.IsLocked && s.Type != SeatType.Disabled));

                    var partlySpareTables = spareTables.Where(t => t.Seats.Any(s => s.IsSelected));

                    ResponseSeat seatToSelect = null;

                    Func<IEnumerable<ResponseTable>, ResponseSeat> seatSelector = t =>
                    {
                        return t.OrderBy(x => x.Id).First().Seats.OrderBy(x => x.Id).First();
                    };

                    if (partlySpareTables.Any())
                    {
                        seatToSelect = seatSelector(partlySpareTables);
                    }
                    else if (spareTables.Any())
                    {
                        seatToSelect = seatSelector(spareTables);
                    }

                    if (seatToSelect != null)
                    {
                        seatToSelect.IsSelected = true;
                        selectionInfo.SelectedObjects.Add(new BusObject { Id = seatToSelect.Id, Type = BusObjectTypes.Seat });
                    }
                }
            }
            else if (selectionInfo.OrderType == OrderType.RegularGroup)
            {
                tourInfo.ForEach(t => t.Seats.ForEach(s => s.IsLocked = false));

                var busModel = ConvertToBusModel(tourInfo, selectionInfo);

                busModel.SelectionSwitch(selectionInfo);

                tables = tourInfo.Select(p => ConvertToResponseModel(p, busModel)).ToList();

                selectionResult.AvailableObjects = tables.SelectMany(x => x.Seats).Select(x => new BusObject { Id = x.Id, Type = BusObjectTypes.Seat }).ToList();

                if (selectionInfo.SelectedObjects.Count >= selectionInfo.GuestCount)
                {
                    selectionResult.AvailableObjects.Clear();
                }
            }

            foreach (var table in tables)
            {
                table.IsAvailable = selectionResult.AvailableObjects.Any(p => p.Type == BusObjectTypes.Table && p.Id == table.Id);

                table.Seats.ForEach(s => s.IsAvailable = selectionResult.AvailableObjects.Any(p => p.Type == BusObjectTypes.Seat && p.Id == s.Id));
            }

            var orderInfo = await GetOrderInfoAsync(selectionInfo.TourId, selectionInfo.GuestCount, tables, selectionInfo.SelectedObjects);

            return new ResponseSelection
                {
                    IsSuccess = true,
                    Selection = selectionInfo,
                    Tables    = tables,
                    OrderInfo = orderInfo
            };
        }

        public async Task<ResponseSelection> GetSelectableBusModelForOrderAsync(SelectionInfo selectionInfo)
        {
            var tourInfo = await _selectionRepository.GetTourInfoAsync(selectionInfo.TourId);
            var order = await IoC.GetRequiredService<IOrderRepository>().GetAsync(selectionInfo.OrderId.Value, true);
            var selectionResult = new SelectionResult { AvailableObjects = new List<BusObject>() };
            var tables = new List<ResponseTable>();

            tourInfo.ForEach(t => t.Seats.ForEach(s => {
                // Занятые места текущего заказа не считаем не доступными для выбора
                if (order.Seats.Any(os => os.SeatId == s.Id))
                {
                    s.IsLocked = false;
                }

                s.IsLocked = s.IsLocked.HasValue 
                    ? s.IsLocked 
                    : false;
            }));

            var busModel = ConvertToBusModel(tourInfo, selectionInfo);

            busModel.SelectionSwitch(selectionInfo);
            tables = tourInfo.Select(p => ConvertToResponseModel(p, busModel)).ToList();
            selectionResult.AvailableObjects = tables.SelectMany(x => x.Seats).Select(x => new BusObject { Id = x.Id, Type = BusObjectTypes.Seat }).ToList();

            if (selectionInfo.SelectedObjects.Count >= selectionInfo.GuestCount)
            {
                selectionResult.AvailableObjects.Clear();
            }

            foreach (var table in tables)
            {
                table.IsAvailable = selectionResult.AvailableObjects.Any(p => p.Type == BusObjectTypes.Table && p.Id == table.Id);
                table.Seats.ForEach(s => s.IsAvailable = selectionResult.AvailableObjects.Any(p => p.Type == BusObjectTypes.Seat && p.Id == s.Id));
            }

            var orderInfo = await GetOrderInfoAsync(selectionInfo.TourId, selectionInfo.GuestCount, tables, selectionInfo.SelectedObjects);

            return new ResponseSelection
            {
                IsSuccess = true,
                Selection = selectionInfo,
                Tables = tables,
                OrderInfo = orderInfo
            };
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        private void Init()
        {
            for (var pos = 1; pos <= 8; pos++)
            {
                _ruleSelectorWrappersTable[pos] = new RuleSelectorWrapper();
                _ruleSelectorWrappersSeat[pos]  = new RuleSelectorWrapper();
            }

            _ruleSelectorWrappersTable[1].Add(0, null, _rsTable1);
            _ruleSelectorWrappersTable[2].Add(0, null, _rsTable2);
            _ruleSelectorWrappersTable[3].Add(0, null, _rsTable3);
            _ruleSelectorWrappersTable[4].Add(0, null, _rsTable4);
            _ruleSelectorWrappersTable[5].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[5].Add(4, null, _rsTable1);
            _ruleSelectorWrappersTable[6].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[6].Add(4, null, _rsTable2);
            _ruleSelectorWrappersTable[7].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[7].Add(4, null, _rsTable3);
            _ruleSelectorWrappersTable[8].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[8].Add(4, null, _rsTable4);

            _ruleSelectorWrappersSeat[1].Add(0, null, _rsSeat1);
            _ruleSelectorWrappersSeat[2].Add(0, null, _rsSeat2);
            _ruleSelectorWrappersSeat[3].Add(0, 1, _rsTable2); // 3 - Table2 + Seat1
            _ruleSelectorWrappersSeat[3].Add(2, null, _rsSeat1);
            _ruleSelectorWrappersSeat[4].Add(0, 1, _rsTable2); // 4 - Table2 + Table2
            _ruleSelectorWrappersSeat[4].Add(2, null, _rsTable2);
            _ruleSelectorWrappersSeat[5].Add(0, 1, _rsTable2); // 5 - Table2 + Table2 + Seat1
            _ruleSelectorWrappersSeat[5].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[5].Add(4, null, _rsSeat1);
            _ruleSelectorWrappersSeat[6].Add(0, 1, _rsTable2); // 6 - Table2 + Table2 + Table2
            _ruleSelectorWrappersSeat[6].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[6].Add(4, null, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(0, 1, _rsTable2); // 7 - Table2 + Table2 + Table2 + Seat1
            _ruleSelectorWrappersSeat[7].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(4, 5, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(6, null, _rsSeat1);
            _ruleSelectorWrappersSeat[8].Add(0, 1, _rsTable2); // 8 - Table2 + Table2 + Table2 + Table2
            _ruleSelectorWrappersSeat[8].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[8].Add(4, 5, _rsTable2);
            _ruleSelectorWrappersSeat[8].Add(6, null, _rsTable2);

            _ruleSelectors[SelectionVariant.IndividualTable] = new[]
                {
                    new { Count = 1, Wrapper = _ruleSelectorWrappersTable[1] },
                    new { Count = 2, Wrapper = _ruleSelectorWrappersTable[2] },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersTable[3] },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersTable[4] },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersTable[5] },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersTable[6] },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersTable[7] },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersTable[8] },
                }.ToLookup(p => p.Count, p => p.Wrapper);

            _ruleSelectors[SelectionVariant.SharedTable] = new[]
                {
                    new { Count = 1, Wrapper = _ruleSelectorWrappersSeat[1]  },
                    new { Count = 2, Wrapper = _ruleSelectorWrappersSeat[2]  },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersTable[3] },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersSeat[3]  },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersTable[4] },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersSeat[4]  },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersTable[5] },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersSeat[5]  },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersTable[6] },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersSeat[6]  },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersTable[7] },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersSeat[7]  },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersTable[8] },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersSeat[8]  }
                }.ToLookup(p => p.Count, p => p.Wrapper);
        }

        /// <summary>
        /// Попытка выполнить автоматический подбор мест.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="selectionInfo">Параметры подбора мест.</param>
        /// <returns>Признак, удалось ли выполнить автоматический подбор мест.</returns>
        private bool TryAutoSelect(BusModel busModel, SelectionInfo selectionInfo)
        {
            while (true)
            {
                var ruleSelector = GetRuleSelector(busModel, selectionInfo);
                if (ruleSelector == null)
                    return false;

                var ruleAction = ruleSelector.SelectBestRuleAction(busModel, out var path);
                if (ruleAction == null || ruleAction.NoProposals)
                    return false;

                if (!ruleAction.TryAutoSelect(busModel, selectionInfo))
                    return false;

                if (selectionInfo.GuestCount <= busModel.CountSelectedSeats)
                    return true;
            }
        }

        /// <summary>
        /// Применить лучшее правило.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="selectionInfo">Параметры подбора мест.</param>
        /// <returns>Результат подбора мест.</returns>
        private SelectionResult ApplyBestRule(BusModel busModel, SelectionInfo selectionInfo)
        {
            var ruleSelector = GetRuleSelector(busModel, selectionInfo);
            if (ruleSelector == null)
                return new SelectionResult();

            var ruleAction = ruleSelector.SelectBestRuleAction(busModel, out var path);

            var availableObjects = new List<BusObject>();

            var neededSeats = selectionInfo.GuestCount - busModel.CountSelectedSeats;

            if (neededSeats > 0)
            {
                availableObjects = ruleAction.GetAvailableObjects(busModel, neededSeats);

                var tables =
                    busModel.Tables
                        .Where(p =>
                            p.CountSelectedSeats > 0 &&
                            availableObjects.Any(x => x.Type == BusObjectTypes.Seat && p.Seats.Any(s => s.Id == x.Id)))
                        .ToArray();

                if (tables.Any())
                {
                    availableObjects
                        .RemoveAll(p => p.Type == BusObjectTypes.Table || !tables.Any(x => x.Seats.Any(s => s.Id == p.Id)));
                }
            }

            return new SelectionResult
                {
                    AvailableObjects = availableObjects
                };
        }

        /// <summary>
        /// Получить объект для подбора лучшего правила.
        /// </summary>
        /// <param name="busModel"></param>
        /// <param name="selectionInfo"></param>
        /// <returns>Объект для подбора лучшего правила.</returns>
        private RuleSelector GetRuleSelector(BusModel busModel, SelectionInfo selectionInfo)
        {
            var countSelectedSeats = busModel.CountSelectedSeats;

            if (!_ruleSelectors.TryGetValue(selectionInfo.SeatingType, out var wrappers))
                return null;

            var list = wrappers[selectionInfo.GuestCount].ToList();
            if (list == null)
                return null;

            RuleSelector result = null;

            foreach (var wrapper in list)
            {
                if (wrapper == null) continue;

                result = wrapper.GetSelector(countSelectedSeats);
                var ruleAction = result?.SelectBestRuleAction(busModel, out var path);

                if (ruleAction == null) continue;

                if (!ruleAction.NoProposals)
                    break;
            }

            return result;
        }

        private async Task<ResponseSelectionOrderInfo> GetOrderInfoAsync(int tourId, int guestsCount, List<ResponseTable> tables, List<BusObject> selectedObjects)
        {
            var result = new ResponseSelectionOrderInfo();

            if (selectedObjects?.Any() != true)
                return result;

            var tablePositions = await _selectionRepository.GetBusObjectsPositionsAsync(tourId);

            foreach (var tablePosition in tablePositions)
            {
                var ids = new List<BusObject> { new BusObject { Type = BusObjectTypes.Table, Id = tablePosition.Id } };
                ids.AddRange(tablePosition.Childs.Select(p => new BusObject { Type = BusObjectTypes.Seat, Id = p.Id }));

                if (!ids.Any(p => Contains(selectedObjects, p))) continue;

                var xPositions = new List<short> { tablePosition.X };
                var yPositions = new List<short> { tablePosition.Y };

                xPositions.AddRange(tablePosition.Childs.Select(p => p.X));
                yPositions.AddRange(tablePosition.Childs.Select(p => p.Y));

                var minX = xPositions.Min();
                var minY = yPositions.Min();

                var table = tables.FirstOrDefault(p => p.Id == tablePosition.Id);
                if (table == null) continue;

                var orderTable = new ResponseSelectionOrderTableInfo
                    {
                        Id = tablePosition.Id,
                        X  = (short)(tablePosition.X - minX),
                        Y  = (short)(tablePosition.Y - minY)
                    };

                foreach (var seatPosition in tablePosition.Childs)
                {
                    var seat = table.Seats.FirstOrDefault(p => p.Id == seatPosition.Id);
                    if (seat == null) continue;

                    var isSelected = seat.IsSelected || (table.IsSelected && guestsCount > 0);
                    if (isSelected)
                    {
                        guestsCount--;

                        var guest = new ResponseSelectionMenuGuestInfo 
                            {
                                SeatId       = seatPosition.Id,
                                SeatFullName = $"{table.Number} {seat.Number}"
                            };
                        result.Guests.Add(guest);
                    }

                    var orderSeat = new ResponseSelectionOrderSeatInfo
                        {
                            Id         = seatPosition.Id,
                            X          = (short)(seatPosition.X - minX),
                            Y          = (short)(seatPosition.Y - minY),
                            IsSelected = isSelected
                        };

                    orderTable.Seats.Add(orderSeat);
                }

                result.Tables.Add(orderTable);
            }

            return result;
        }

        private bool Contains(List<BusObject> selectedObjects, BusObject obj)
        {
            if (selectedObjects == null || obj == null) return false;

            return selectedObjects.Any(p => p.Type == obj.Type && p.Id == obj.Id);
        }

        /// <summary>
        /// Преобразование в модель стола для ответа на запрос с клиента.
        /// </summary>
        /// <param name="entity">Сущность - стол для подбора.</param>
        /// <param name="busModel">Модель автобуса.</param>
        /// <returns>Модель стола для ответа на запрос с клиента.</returns>
        private ResponseTable ConvertToResponseModel(SelectionTable entity, BusModel busModel)
        {
            var table = busModel.Tables.FirstOrDefault(p => p.Id == entity.Id);

            var result = new ResponseTable
                {
                    Id          = entity.Id,
                    Number      = $"{(entity.TableCategory == TableCategories.Vip ? "VIP " : "")}{entity.Number}",
                    IsLocked    = table == null,
                    IsSelected  = table?.IsSelected ?? false,
                    IsAvailable = false,
                    Seats       = entity.Seats.Select(p => ConvertToResponseModel(p, table)).ToList()
                };

            return result;
        }

        /// <summary>
        /// Преобразование в модель места для ответа на запрос с клиента.
        /// </summary>
        /// <param name="entity">Сущность - место для подбора.</param>
        /// <param name="table">Модель стола.</param>
        /// <returns>Модель места для ответа на запрос с клиента.</returns>
        private ResponseSeat ConvertToResponseModel(SelectionSeat entity, TableModel table)
        {
            var seat = table?.Seats.FirstOrDefault(p => p.Id == entity.Id);

            var result = new ResponseSeat
                {
                    Id          = entity.Id,
                    Number      = entity.Name,
                    IsLocked    = table == null || (entity.IsLocked ?? false),
                    IsSelected  = seat?.IsSelected ?? false,
                    IsAvailable = false,
                    OrderId     = entity.OrderId,
                    Type        = entity.Type
                };

            return result;
        }

        /// <summary>
        /// Преобразование в модель автобуса.
        /// </summary>
        /// <param name="tables">Коллекция столов.</param>
        /// <param name="selection">Информация о выборе пользователя.</param>
        /// <returns>Модель автобуса.</returns>
        private BusModel ConvertToBusModel(List<SelectionTable> tables, SelectionInfo selection)
        {
            var result = new BusModel
                {
                    FirstFloor  = new FloorModel { Tables = new List<TableModel>() },
                    SecondFloor = new FloorModel { Tables = new List<TableModel>() }
                };

            foreach (var table in tables)
            {
                if (table.TableCategory == TableCategories.Wheelchair) continue;

                var floor = table.Floor == 1 ? result.FirstFloor : result.SecondFloor;

                floor.Tables.Add(ConvertToBusModel(table, selection));
            }

            return result;
        }

        /// <summary>
        /// Преобразование в модель стола.
        /// </summary>
        /// <param name="entity">Сущность - стол для подбора.</param>
        /// <param name="selection">Информация о выборе пользователя.</param>
        /// <returns>Модель стола.</returns>
        private TableModel ConvertToBusModel(SelectionTable entity, SelectionInfo selection)
        {
            var result = new TableModel
                {
                    Id         = entity.Id,
                    Number     = $"{(entity.TableCategory == TableCategories.Vip ? "VIP " : "")}{entity.Number}",
                    IsSelected = selection.SelectedObjects.Any(p => p.Type == BusObjectTypes.Table && p.Id == entity.Id),
                    IsFirstRow = entity.TableLocation == TableLocations.FirstRow,
                    IsLastRow  = entity.TableLocation == TableLocations.LastRow,
                    Seats      = entity.Seats.Select(p => ConvertToBusModel(p, selection)).ToList()
                };

            result.Type = entity.Seats.Count switch
            {
                1 => TableTypes.One,
                2 => TableTypes.Two,
                4 => TableTypes.Four,
                _ => TableTypes.Two
            };

            return result;
        }

        /// <summary>
        /// Преобразование в модель места.
        /// </summary>
        /// <param name="entity">Сущность - место для подбора.</param>
        /// <param name="selection">Информация о выборе пользователя.</param>
        /// <returns>Модель места.</returns>
        private SeatModel ConvertToBusModel(SelectionSeat entity, SelectionInfo selection)
        {
            var result = new SeatModel
                {
                    Id         = entity.Id, 
                    Number     = entity.Name,
                    IsLocked   = entity.IsLocked ?? false,
                    IsSelected = selection.SelectedObjects.Any(p => p.Type == BusObjectTypes.Seat && p.Id == entity.Id),
                    Type       = entity.Type
                };

            return result;
        }
    }
}
