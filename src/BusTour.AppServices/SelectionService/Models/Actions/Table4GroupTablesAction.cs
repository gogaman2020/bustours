using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Selection;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предложим два стола в первом ряду и два стола в последнем ряду и столы на четверых целиком.
    /// </summary>
    public class Table4GroupTablesAction : BaseRuleAction
    {
        /// <inheritdoc/>
        protected override bool IsTableAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            var selectedInFirstRow =
                busModel.Tables.Where(p => p.IsFirstRow && p.IsSelected).Select(p => p.CountSelectedSeats).DefaultIfEmpty(0).Sum();
            var selectedInLastRow =
                busModel.Tables.Where(p => p.IsLastRow && p.IsSelected).Select(p => p.CountSelectedSeats).DefaultIfEmpty(0).Sum();

            bool allowFirstRowGroup =
                (neededSeats + selectedInFirstRow) >= 3 &&
                busModel.Tables.Any(p => p.IsFirstRow && p.IsFree) && 
                busModel.Tables.Count(p => p.IsFirstRow && (p.IsFree || p.IsSelected)) >= 2;
            bool allowLastRowGroup =
                (neededSeats + selectedInLastRow) >= 3 &&
                busModel.Tables.Any(p => p.IsLastRow && p.IsFree) &&
                busModel.Tables.Count(p => p.IsLastRow && (p.IsFree || p.IsSelected)) >= 2;

            bool isFirstRowSelected = allowFirstRowGroup && busModel.Tables.Any(p => p.IsFirstRow && p.IsSelected);
            bool isLastRowSelected = allowLastRowGroup && busModel.Tables.Any(p => p.IsLastRow && p.IsSelected);

            return
                (table.IsFirstRow && table.IsFree && allowFirstRowGroup && !isLastRowSelected) ||
                (table.IsLastRow && table.IsFree && allowLastRowGroup && !isFirstRowSelected) ||
                (table.Type == TableTypes.Four && table.IsFree && !isFirstRowSelected && !isLastRowSelected);
        }

        /// <inheritdoc/>
        protected override BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            if (!availableTables.Any()) return new BusObject[0];

            var firstRowTables = 
                availableTables
                    .Where(p => p.Type == TableTypes.Two && p.IsFirstRow)
                    .OrderBy(p => p.Number)
                    .Take(2)
                    .ToArray();

            var lastRowTables =
                availableTables
                    .Where(p => p.Type == TableTypes.Two && p.IsLastRow)
                    .OrderBy(p => p.Number)
                    .Take(2)
                    .ToArray();

            var firstTable =
                availableTables
                    .Where(p => p.Type == TableTypes.Four)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();

            var tables =
                firstRowTables.Length == 2
                    ? firstRowTables
                    : lastRowTables.Length == 2
                        ? lastRowTables
                        : firstTable != null
                            ? new AutoSelectTable[] { firstTable }
                            : null;

            return
                tables != null
                    ? tables.Select(p => new BusObject { Type = BusObjectTypes.Table, Id = p.Id }).ToArray()
                    : new BusObject[0];
        }
    }
}
