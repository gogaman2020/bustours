using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Selections;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService.Queries
{

    /// <summary>
    /// Получаем модель автобуса для заказа.
    /// </summary>
    [InjectAsScoped]
    public class GetOrderBusModelQuery : MediatorQuery<OrderBusModel>
    {
        private readonly int _orderId;

        private readonly IOrderRepository _orderRepository;
        private readonly IBusRepository _busRepository;
        private readonly ISelectionRepository _selectionRepository;

        private Order _order;
        private Bus _bus;
        private List<SelectionTable> _tourInfo;

        private decimal _regularSeatPrice;
        private decimal _vipSeatPrice;

        public GetOrderBusModelQuery(int orderId)
        {
            _orderId = orderId;

            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _busRepository   = IoC.GetRequiredService<IBusRepository>();
            _selectionRepository = IoC.GetRequiredService<ISelectionRepository>();
        }
        
        public override async Task<MediatorCommandResult<OrderBusModel>> ExecuteAsync()
        {
            _order = await _orderRepository.GetAsync(_orderId);
            _tourInfo = await _selectionRepository.GetTourInfoAsync(_order.TourId);
            _bus = await _busRepository.GetAsync(_order.Tour.BusId);

            _regularSeatPrice = _bus.Tables.First(x => !x.IsVip).Seats.First().Price;
            _vipSeatPrice = _bus.Tables.First(x => x.IsVip).Seats.First().Price;

            var model = InitModel();

            //Правила показа, которые применяются последовательно
            var rules = new List<Func<OrderBusModel, OrderBusModel>>
            {
                ApplyOrders,
                ApplyMaxGuestsCount,
                SetFullTableAvailable,
                ForbidOtherFullTables,
                CalcRegularFullTableDiscount,
                VipTablesWithRegularSeats,
                VipTablesWithoutRegularSeats,
                UpgadeToTable,
                SetSelectedToAvailable,
                SetDisabledToUnavailable
            };

            model.TablesRules = new Dictionary<byte, List<dynamic>>();
            model.SeatsRules = new Dictionary<string, List<dynamic>>();

            model.Tables.ForEach(x =>
            {
                model.TablesRules.Add(x.Table.Number, new List<dynamic>());
            });
            model.Seats.ForEach(x =>
            {
                model.SeatsRules.Add($"{x.Seat.Table.Number}-{x.Seat.Name}", new List<dynamic>());
            });

            foreach (var rule in rules)
            {
                model = rule(model);

                model.Tables.ForEach(x =>
                {
                    model.TablesRules[x.Table.Number].Add(new { Rule = rule.Method.Name, x.IsAvailable, x.IsAvailableForUpgrade, x.IsSelected });
                });

                model.Seats.ForEach(x =>
                {
                    model.SeatsRules[$"{x.Seat.Table.Number}-{x.Seat.Name}"].Add(new { Rule = rule.Method.Name, x.IsAvailable, x.IsSelected });
                });
            }

            var test = model.Tables.SelectMany(x => x.Seats).Where(x => x.IsSelected).ToList();

            return Success(model);
        }

        //Заполняем начальную модель по схеме автобуса
        private OrderBusModel InitModel()
        {
            var model = new OrderBusModel();

            var seatPrice = _order.Tour.SeatPrice;
            var vipPrice = _order.Tour.VipPrice;

            model.Tables = _bus.Tables.Select(t => new OrderBusTableModel
            {
                Id = t.Id,
                Price = t.Price,
                AllowedByRules = true,
                InitialPrice = t.Price,
                Table = t,
                Order = _order
            }).ToList();

            model.Tables.ForEach(t => t.Seats = t.Table.Seats.Select(s => {

                var model = new OrderBusSeatModel
                {
                    Id = s.Id,
                    Price = t.IsVip ? vipPrice ?? s.Price : seatPrice ?? s.Price,
                    InitialPrice = s.Price,
                    IsAllowedByRules = true,
                    TableModel = t,
                    Seat = s,
                    Order = _order
                };

                model.Seat.Table = t.Table;

                return model;
                }).ToList());

            return model;
        }

        //Проставляем занятость из других заказов
        private OrderBusModel ApplyOrders(OrderBusModel model)
        {
            var seletecedSeatIds = _order.Seats.Where(x => x.IsEmpty != true).Select(x => x.SeatId).ToList();
            var lockedSeatIds = _tourInfo.SelectMany(x => x.Seats).Where(x => x.IsLocked == true).Select(x => x.Id).ToList();

            model.Seats.ForEach(seat =>
            {
                seat.IsOtherOrdered = lockedSeatIds.Contains(seat.Id) && !seletecedSeatIds.Contains(seat.Id);
                seat.IsSelected     = seletecedSeatIds.Contains(seat.Id);
            });

            return model;
        }

        //Запрет на выбор, если все посажены
        private OrderBusModel ApplyMaxGuestsCount(OrderBusModel model)
        {
            if (_order.Seats.Count >= _order.GuestCount)
            {
                model.Tables.ForEach(x => x.AllowedByRules = false);
                model.Seats.ForEach(x => x.IsAllowedByRules = false);
            }

            return model;
        }

        //Полностью столы доступны, только если нет занятых мест за ними
        private OrderBusModel SetFullTableAvailable(OrderBusModel model)
        {
            model.Tables
                .ForEach(t => t.AllowedByRules = t.AllowedByRules && t.Seats.All(x => x.IsFree));

            return model;
        }

        //Запрещаем выбор другого столы, если есть места за текущим
        private OrderBusModel ForbidOtherFullTables(OrderBusModel model)
        {
            var selecetdTableWithFreeSeats = model.Tables.FirstOrDefault(x => x.Seats.Any(s => s.IsSelected) && x.Seats.Any(s => s.IsFree));

            if (selecetdTableWithFreeSeats != null)
            {
                model.Tables
                    .Where(x => x.Id != selecetdTableWithFreeSeats.Id)
                    .ToList()
                    .ForEach(t => t.AllowedByRules = false);
            }
            return model;
        }

        //Рассчитываем скидку на регулярные столы
        private OrderBusModel CalcRegularFullTableDiscount(OrderBusModel model)
        {
            var discount = 10;

            if (_order.FreeCount > 0)
            {
                foreach(var table in model.Tables.Where(x => x.IsAvailable && !x.Table.IsVip))
                {
                    var freeOnTable = table.Seats.Count(x => x.IsFree);
                    var delta = freeOnTable - _order.FreeCount;
                    if (delta > 0)
                    {
                        table.Price = table.InitialPrice - delta * discount;
                    }
                }
            }

            return model;
        }

        //При наличии других доступных вариантов система автоматически предлагает ВИП места только группам от 5 человек, при этом стоимость указывается, как за обычное место
        private OrderBusModel VipTablesWithRegularSeats(OrderBusModel model)
        {
            if (model.Seats.Any(x => x.IsAvailable && x.IsRegular) && _order.GuestCount >= 5)
            {
                model.Seats.ToList(x => x.IsFree && x.IsVip).ForEach(vip =>
                {
                    vip.Price = _regularSeatPrice;
                });
            }

            return model;
        }

        //В остальных случаях система автоматически предлагает ВИП места только при отсутствии других мест с пометкой того, что это ВИП места
        private OrderBusModel VipTablesWithoutRegularSeats(OrderBusModel model)
        {
            if (!model.Seats.Any(x => x.IsFree && x.IsRegular))
            {
                if (_order.GuestCount == 2 && _order.TableType == SelectionVariant.SharedTable)
                {
                    model.Seats.ToList(x => x.IsFree && x.IsVip).ForEach(s => s.Price = _regularSeatPrice);
                }
                else if (_order.GuestCount.In(2) && _order.TableType == SelectionVariant.IndividualTable)
                {
                    model.Tables.ToList(x => x.IsVip && x.IsFree).ForEach(s => s.Price = _regularSeatPrice * 3);
                }
                else if (_order.GuestCount.In(3) && _order.TableType == SelectionVariant.IndividualTable)
                {
                    model.Tables.ToList(x => x.IsVip && x.IsFree).ForEach(s => s.Price = _vipSeatPrice * 3);
                }
            }

            return model;
        }

        //Пассажир имеет право выбрать ВИП место в режиме апгрейда мест только в том случае, если он берет весь стол целиком
        private OrderBusModel LockVipSeats(OrderBusModel model)
        {
            model.Seats.ToList(x => x.IsVip).ForEach(s => s.IsAllowedByRules = false);

            return model;
        }

        //Пассажир имеет право апгрейднуть место до стола
        private OrderBusModel UpgadeToTable(OrderBusModel model)
        {
            var tables = model.Tables
                .Where(x => x.Seats.Any(x => x.IsSelected))
                .Where(x => x.Seats.Any(x => x.IsFree || x.IsSelected))
                .ToList();

            tables.ForEach(t =>
            {
                model.Tables.FirstOrDefault(x => x == t).IsAvailableForUpgrade = true;
            });

            return model;
        }

        //Выбранное - доступно
        private OrderBusModel SetSelectedToAvailable(OrderBusModel model)
        {
            model.Tables.SelectMany(x => x.Seats).ToList().ForEach(s =>
            {
                s.IsAllowedByRules = s.IsAllowedByRules || s.IsSelected;
            });

            return model;
        }

        //Инвалиды - не доступно
        private OrderBusModel SetDisabledToUnavailable(OrderBusModel model)
        {
            model.Tables.SelectMany(x => x.Seats).ToList().ForEach(s =>
            {
                s.IsAllowedByRules = s.IsAllowedByRules && s.Seat.Type != SeatType.Disabled;
            });

            return model;
        }
    }
}
