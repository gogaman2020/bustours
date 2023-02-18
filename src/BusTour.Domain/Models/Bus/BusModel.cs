using BusTour.Domain.Enums;
using BusTour.Domain.Models.Responses;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Models.Bus
{
    /// <summary>
    /// Модель автобуса.
    /// </summary>
    public class BusModel
    {
        /// <summary>
        /// Модель первого этажа.
        /// </summary>
        public FloorModel FirstFloor { get; set; }

        /// <summary>
        /// Модель второго этажа.
        /// </summary>
        public FloorModel SecondFloor { get; set; }

        /// <summary>
        /// Коллекция моделей этажей.
        /// </summary>
        public IEnumerable<FloorModel> Floors
        {
            get
            {
                yield return FirstFloor;
                yield return SecondFloor;
            }
        }

        /// <summary>
        /// Коллекция столов.
        /// </summary>
        public IEnumerable<TableModel> Tables => Floors.SelectMany(p => p.Tables);

        public List<BusObject> Recommended { get; } = new List<BusObject>();

        /// <summary>
        /// Количество выбранных мест.
        /// </summary>
        public int CountSelectedSeats => Tables.Sum(p => p.CountSelectedSeats);

        /// <summary>
        /// Проверка типа правила для стола на двоих.
        /// </summary>
        /// <param name="ruleType">Тип правила для стола на двоих.</param>
        /// <returns>Признак, выполняется ли тип правила для стола на двоих.</returns>
        public bool CheckRuleType(RuleTypesTwoSeats ruleType)
        {
            switch (ruleType)
            {
                case RuleTypesTwoSeats.AlwaysTrue:
                    return true;

                case RuleTypesTwoSeats.TableFirstRow:
                    return Tables.Any(p => p.Type == TableTypes.Two && p.IsFirstRow && p.IsAvailable);

                case RuleTypesTwoSeats.MoreFreeTable:
                    return Tables.Count(p => p.Type == TableTypes.Two && !p.IsFirstRow && p.IsAvailable) > 1;

                case RuleTypesTwoSeats.FreeTable:
                    return Tables.Any(p => p.Type == TableTypes.Two && !p.IsFirstRow && p.IsAvailable);

                case RuleTypesTwoSeats.OneFreeSeat:
                    return Tables.Any(p => p.Type == TableTypes.Two && !p.IsFirstRow && p.CountAvailableSeats == 1);
            }

            return false;
        }

        /// <summary>
        /// Проверка типа правила для стола на четверых.
        /// </summary>
        /// <param name="ruleType">Тип правила для стола на четверых.</param>
        /// <returns>Признак, выполняется ли тип правила для стола на четверых.</returns>
        public bool CheckRuleType(RuleTypesFourSeats ruleType)
        {
            switch (ruleType)
            {
                case RuleTypesFourSeats.MoreFreeTable:
                    return Tables.Count(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.IsAvailable) > 1;

                case RuleTypesFourSeats.FreeTable:
                    return Tables.Any(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.IsAvailable);

                case RuleTypesFourSeats.ThreeFreeSeats:
                    return Tables.Any(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.CountAvailableSeats == 3);

                case RuleTypesFourSeats.TwoFreeSeats:
                    return Tables.Any(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.CountAvailableSeats == 2);

                case RuleTypesFourSeats.OneFreeSeat:
                    return Tables.Any(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.CountAvailableSeats == 1);

                case RuleTypesFourSeats.GroupTables:
                    {
                        var freeTable = Tables.Any(p => p.Type == TableTypes.Four && !p.IsFirstRow && p.IsAvailable);

                        var tablesFirstRow = Tables.Where(p => p.Type == TableTypes.Two && p.IsFirstRow).ToArray();
                        var tablesLastRow = Tables.Where(p => p.Type == TableTypes.Two && p.IsLastRow).ToArray();

                        var availableFirstRowCount = tablesFirstRow.Count(p => p.IsFree);
                        var availableLastRowCount = tablesLastRow.Count(p => p.IsFree);

                        var selectedFirstRowCount = tablesFirstRow.Count(p => p.IsSelected);
                        var selectedLastRowCount = tablesLastRow.Count(p => p.IsSelected);

                        var isFreeFirstRow = availableFirstRowCount > 0 && (availableFirstRowCount + selectedFirstRowCount) >= 2;
                        var isFreeLastRow = availableLastRowCount > 0 && (availableLastRowCount + selectedLastRowCount) >= 2;

                        return freeTable || isFreeFirstRow || isFreeLastRow;
                    }
            }

            return false;
        }

        /// <summary>
        /// Смена состояния выбора.
        /// </summary>
        /// <param name="selectionInfo">Информация о выборе пользователя.</param>
        public void SelectionSwitch(SelectionInfo selectionInfo)
        {
            if (selectionInfo.ClickedObject == null) return;

            if (selectionInfo.SelectedObjects == null)
                selectionInfo.SelectedObjects = new List<BusObject>();

            switch (selectionInfo.ClickedObject.Type)
            {
                case BusObjectTypes.Table:
                    {
                        var table = GetTable(selectionInfo.ClickedObject.Id);
                        if (table == null || table.IsLocked) return;

                        bool isSelected = selectionInfo.SelectedObjects.Any(p => p.Type == BusObjectTypes.Table && p.Id == table.Id);

                        if (isSelected)
                        {
                            table.IsSelected = false;
                            selectionInfo.SelectedObjects.RemoveAll(p => p.Type == BusObjectTypes.Table && p.Id == table.Id);
                            selectionInfo.SelectedObjects.RemoveAll(p => p.Type == BusObjectTypes.Seat && table.Seats.Any(s => s.Id == p.Id));
                        }
                        else
                        {
                            table.IsSelected = true;
                            selectionInfo.SelectedObjects.Add(new BusObject { Type = BusObjectTypes.Table, Id = table.Id });
                        }
                    }
                    break;
                case BusObjectTypes.Seat:
                    {
                        var seat = GetSeat(selectionInfo.ClickedObject.Id);
                        if (seat == null || seat.IsLocked) return;

                        bool isSelected = selectionInfo.SelectedObjects.Any(p => p.Type == BusObjectTypes.Seat && p.Id == seat.Id);

                        if (isSelected)
                        {
                            seat.IsSelected = false;
                            selectionInfo.SelectedObjects.RemoveAll(p => p.Type == BusObjectTypes.Seat && p.Id == seat.Id);
                        }
                        else
                        {
                            var table = Tables.FirstOrDefault(x => x.Seats.Any(s => s.Id == seat.Id));
                            if (table != null && !selectionInfo.SelectedObjects.Any(x => x.Type == BusObjectTypes.Table && x.Id == table.Id))
                            {
                                seat.IsSelected = true;
                                selectionInfo.SelectedObjects.Add(new BusObject { Type = BusObjectTypes.Seat, Id = seat.Id });
                            }
                        }
                    }
                    break;
            }

            selectionInfo.ClickedObject = null;
        }

        public void SaveSelectedToRecommended()
        {
            Recommended.Clear();

            foreach (var table in Tables)
            {
                if (table.IsSelected)
                    Recommended.Add(new BusObject { Type = BusObjectTypes.Table, Id = table.Id });

                if (table.Seats == null) continue;

                foreach (var seat in table.Seats)
                {
                    if (seat.IsSelected)
                        Recommended.Add(new BusObject { Type = BusObjectTypes.Seat, Id = seat.Id });
                }
            }
        }

        public void ClearRecommended()
        {
            Recommended.Clear();
        }

        /// <summary>
        /// Отменить выбор пользователя.
        /// </summary>
        public void UnSelectAll()
        {
            foreach (var table in Tables)
            {
                table.IsSelected = false;

                if (table.Seats == null) continue;

                table.Seats.ForEach(p => p.IsSelected = false);
            }
        }

        /// <summary>
        /// Получение стола.
        /// </summary>
        /// <param name="id">Идентификатор стола.</param>
        /// <returns>Стол.</returns>
        protected TableModel GetTable(int id)
        {
            var table = Tables.FirstOrDefault(p => p.Id == id);
            return table;
        }

        /// <summary>
        /// Получение места.
        /// </summary>
        /// <param name="id">Идентификатор места.</param>
        /// <returns>Место.</returns>
        protected SeatModel GetSeat(int id)
        {
            var seat = Tables.SelectMany(p => p.Seats).FirstOrDefault(p => p.Id == id);
            return seat;
        }
    }
}
