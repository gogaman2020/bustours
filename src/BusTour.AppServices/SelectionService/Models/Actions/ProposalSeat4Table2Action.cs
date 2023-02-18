using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Selection;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предложим места за столом на четверых и столы на двоих целиком.
    /// </summary>
    public class ProposalSeat4Table2Action : BaseRuleAction
    {
        /// <summary>
        /// Необходимое количество свободных мест за столом на четверых.
        /// </summary>
        public int AvailableSeats4 { get; set; }

        /// <inheritdoc/>
        protected override bool IsTableAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return table.Type == TableTypes.Two && table.IsFree;
        }

        /// <inheritdoc/>
        protected override bool IsSeatsAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            return table.Type == TableTypes.Four && table.CountAvailableSeats == AvailableSeats4;
        }

        /// <inheritdoc/>
        protected override BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            if (!availableTables.Any() && !availableSeats.Any()) return new BusObject[0];

            var firstSeat =
                availableSeats
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTableTwoSeat =
                availableTables
                    .Where(p => !p.IsFirstRow)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTableTwoSeatVip =
                availableTables
                    .Where(p => p.IsFirstRow)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTable = firstTableTwoSeat ?? firstTableTwoSeatVip;

            return
                firstSeat != null
                    ? firstSeat.Seats.Select(p => new BusObject { Type = BusObjectTypes.Seat, Id = p.Id }).ToArray()
                    : firstTable != null
                        ? new BusObject[] { new BusObject { Type = BusObjectTypes.Table, Id = firstTable.Id } }
                        : new BusObject[0];
        }
    }
}
