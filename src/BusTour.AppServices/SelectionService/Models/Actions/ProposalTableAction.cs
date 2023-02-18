using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Selection;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предложим столы на двоих и/или на четверых.
    /// </summary>
    public class ProposalTableAction : BaseRuleAction
    {
        /// <summary>
        /// Тип стола.
        /// </summary>
        public ActionTableTypes TableType { get; set; }

        /// <inheritdoc/>
        protected override bool IsTableAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            if (!table.IsAvailable) return false;

            switch (TableType)
            {
                case ActionTableTypes.Two:        return table.Type == TableTypes.Two;
                case ActionTableTypes.Four:       return table.Type == TableTypes.Four;
                case ActionTableTypes.TwoAndFour: return table.Type == TableTypes.Two || table.Type == TableTypes.Four;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            if (!availableTables.Any()) return new BusObject[0];

            var firstTwoSeatTable = 
                availableTables
                    .Where(p => p.Type == TableTypes.Two && !p.IsFirstRow)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTwoSeatTableVip = 
                availableTables
                    .Where(p => p.Type == TableTypes.Two && p.IsFirstRow)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstFourSeatTable =
                availableTables
                    .Where(p => p.Type == TableTypes.Four)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();

            AutoSelectTable firstTable = null;

            switch (TableType)
            {
                case ActionTableTypes.Two:
                    {
                        firstTable = firstTwoSeatTable ?? firstTwoSeatTableVip;
                        break;
                    }
                case ActionTableTypes.Four:
                    {
                        firstTable = firstFourSeatTable;
                        break;
                    };
                case ActionTableTypes.TwoAndFour:
                    {
                        firstTable = firstTwoSeatTable ?? firstTwoSeatTableVip ?? firstFourSeatTable;
                        break;
                    }
            }

            return 
                firstTable != null 
                    ? new BusObject[] { new BusObject { Type = BusObjectTypes.Table, Id = firstTable.Id } } 
                    : new BusObject[0];
        }
    }
}
