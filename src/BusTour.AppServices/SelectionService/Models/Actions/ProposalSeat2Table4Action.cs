using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Selection;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предлагаем место за столом на двоих или стол на четверых целиком.
    /// </summary>
    public class ProposalSeat2Table4Action : BaseRuleAction
    {
        /// <inheritdoc/>
        protected override bool IsTableAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return table.Type == TableTypes.Four && table.IsFree;
        }

        /// <inheritdoc/>
        protected override bool IsSeatsAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return table.Type == TableTypes.Two && table.CountAvailableSeats == 1;
        }

        /// <inheritdoc/>
        protected override BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            if (!availableTables.Any() && !availableSeats.Any()) return new BusObject[0];

            var firstSeat =
                availableSeats
                    .OrderBy(p => p.Number)
                    .FirstOrDefault()
                    ?.Seats
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTable =
                availableTables
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();

            return
                firstSeat != null
                    ? new BusObject[] { new BusObject { Type = BusObjectTypes.Seat, Id = firstSeat.Id } }
                    : firstTable != null
                        ? new BusObject[] { new BusObject { Type = BusObjectTypes.Table, Id = firstTable.Id } }
                        : new BusObject[0];
        }
    }
}
