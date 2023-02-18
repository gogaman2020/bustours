using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Базовое правило
    /// </summary>
    public class BaseRuleAction
    {
        /// <summary>
        /// Тип действия.
        /// </summary>
        public ActionTypes Type { get; set; }

        /// <summary>
        /// Признак, что нет предложения.
        /// </summary>
        public virtual bool NoProposals => false;

        /// <summary>
        /// Получить список доступных для выбора объектов.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="neededSeats">Необходимое количество мест.</param>
        /// <returns>Список доступных для выбора объектов</returns>
        public List<BusObject> GetAvailableObjects(BusModel busModel, int neededSeats)
        {
            var result = new List<BusObject>();

            foreach (var table in busModel.Tables)
            {
                if (IsTableAvailable(busModel, table, neededSeats))
                {
                    result.Add(new BusObject { Type = BusObjectTypes.Table, Id = table.Id });
                    continue;
                }

                if (!IsSeatsAvailable(busModel, table, neededSeats)) continue;

                if (table.IsFirstRow && table.IsFree)
                {
                    result.Add(new BusObject { Type = BusObjectTypes.Table, Id = table.Id });
                    continue;
                }

                var availableSeats = table.Seats.Where(p => p.IsAvailable).ToList();

                if (table.Type == TableTypes.Four && table.CountSelectedSeats == 1)
                {
                    var selectedSeat = table.Seats.FirstOrDefault(p => p.IsSelected);

                    switch (selectedSeat?.Number)
                    {
                        case "A":
                        case "D":
                            availableSeats.RemoveAll(p => new[] { "A", "D" }.Contains(p.Number));
                            break;
                        case "B":
                        case "C":
                            availableSeats.RemoveAll(p => new[] { "B", "C" }.Contains(p.Number));
                            break;
                    }
                }

                result.AddRange(availableSeats.Select(p => new BusObject { Type = BusObjectTypes.Seat, Id = p.Id }));
            }

            return result;
        }

        /// <summary>
        /// Проверка, доступен ли для выбора указанный стол.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="table">Модель стола.</param>
        /// <param name="neededSeats">Необходимое количество мест.</param>
        /// <returns>Признак, доступен ли указанный стол для выбора.</returns>
        protected virtual bool IsTableAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return false;
        }

        /// <summary>
        /// Проверка, доступны ли для выбора места за указанным столом.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="table">Модель стола.</param>
        /// <param name="neededSeats">Необходимое количество мест.</param>
        /// <returns>Признак, доступны ли места за указанным столом для выбора.</returns>
        protected virtual bool IsSeatsAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return false;
        }

        /// <summary>
        /// Попытка выполнить автоматический подбор мест.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="selectionInfo">Параметры подбора мест.</param>
        /// <returns>Признак, удалось ли выполнить автоматический подбор мест.</returns>
        public bool TryAutoSelect(BusModel busModel, SelectionInfo selectionInfo)
        {
            var neededSeats = selectionInfo.GuestCount - busModel.CountSelectedSeats;
            var availableObjects = GetAvailableObjects(busModel, neededSeats);

            if (!availableObjects.Any()) return false;

            var availableTables = new List<AutoSelectTable>();
            var availableSeats = new List<AutoSelectTable>();

            foreach (var table in busModel.Tables)
            {
                AutoSelectTable autoSelectModel = null;

                if (availableObjects.Any(x => x.Type == BusObjectTypes.Table && x.Id == table.Id))
                {
                    autoSelectModel = ConvertToAutoSelectModel(table);
                    availableTables.Add(autoSelectModel);
                }

                var seats = table.Seats.Where(p => availableObjects.Any(x => x.Type == BusObjectTypes.Seat && x.Id == p.Id)).ToArray();

                if (seats.Any())
                {
                    autoSelectModel ??= ConvertToAutoSelectModel(table);

                    autoSelectModel.Seats.AddRange(seats.Select(ConvertToAutoSelectModel));

                    availableSeats.Add(autoSelectModel);
                }
            }

            var autoSelectedObjects = GetAutoSelectedObjects(availableTables, availableSeats, neededSeats);
            if (!autoSelectedObjects.Any()) return false;

            foreach (var autoSelectedObject in autoSelectedObjects)
            {
                selectionInfo.ClickedObject = autoSelectedObject;
                busModel.SelectionSwitch(selectionInfo);
            }

            return true;
        }

        /// <summary>
        /// Получение списка объектов, которые должны быть выбраны в автоматическом режиме.
        /// </summary>
        /// <param name="availableTables">Доступные для выбора столы.</param>
        /// <param name="availableSeats">Столы, за которыми есть доступные для выбора отдельные места.</param>
        /// <param name="neededSeats">Необходимое количество мест.</param>
        /// <returns>Коллекция объектов для автоматического выбора.</returns>
        protected virtual BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            return new BusObject[0];
        }

        /// <summary>
        /// Преобразование в модель стола для автоматического выбора.
        /// </summary>
        /// <param name="model">Модель стола.</param>
        /// <returns>Модель стола для автоматического выбора</returns>
        private AutoSelectTable ConvertToAutoSelectModel(TableModel model)
        {
            var result = new AutoSelectTable
                {
                    Id         = model.Id,
                    Number     = int.TryParse(model.Number, out var value) ? value : 0,
                    Type       = model.Type,
                    IsFirstRow = model.IsFirstRow,
                    IsLastRow  = model.IsLastRow,
                    Seats      = new List<AutoSelectSeat>()
                };

            return result;
        }

        /// <summary>
        /// Преобразование в модель места для автоматического выбора.
        /// </summary>
        /// <param name="model">Модель места.</param>
        /// <returns>Модель места для автоматического выбора</returns>
        private AutoSelectSeat ConvertToAutoSelectModel(SeatModel model)
        {
            var result = new AutoSelectSeat
                {
                    Id     = model.Id,
                    Number = model.Number
                };

            return result;
        }
    }
}
