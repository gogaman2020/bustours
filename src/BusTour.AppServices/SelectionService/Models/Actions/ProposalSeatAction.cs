using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Selection;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предложим места за столами на двоих и/или на четверых
    /// </summary>
    public class ProposalSeatAction : BaseRuleAction
    {
        /// <summary>
        /// Тип стола.
        /// </summary>
        public ActionTableTypes SeatType { get; set; }

        /// <summary>
        /// Необходимое количество мест за столом на двоих.
        /// </summary>
        public int AvailableSeats2 { get; set; }

        /// <summary>
        /// Необходимое количество мест за столом на четверых.
        /// </summary>
        public int AvailableSeats4 { get; set; }

        /// <inheritdoc/>
        protected override bool IsSeatsAvailable(BusModel busModel, TableModel table, int neededSeats)
        {
            if (table.IsFirstRow) return false;

            if (neededSeats == 1 && table.Type == TableTypes.One)
            {
                return table.CountAvailableSeats > 0;
            }

            if (SeatType == ActionTableTypes.Two && table.Type == TableTypes.Four) return false;
            if (SeatType == ActionTableTypes.Four && table.Type == TableTypes.Two) return false;

            var availableSeats = table.Type == TableTypes.Two ? AvailableSeats2 : AvailableSeats4;

            return (table.CountAvailableSeats + table.CountSelectedSeats) == availableSeats;
        }

        /// <inheritdoc/>
        protected override BusObject[] GetAutoSelectedObjects(List<AutoSelectTable> availableTables, List<AutoSelectTable> availableSeats, int neededSeats)
        {
            if (!availableSeats.Any()) return new BusObject[0];

            var firstOneSeatTable =
                availableSeats
                    .Where(p => p.Type == TableTypes.One)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstTwoSeatTable =
                availableSeats
                    .Where(p => p.Type == TableTypes.Two)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();
            var firstFourSeatTable =
                availableSeats
                    .Where(p => p.Type == TableTypes.Four)
                    .OrderBy(p => p.Number)
                    .FirstOrDefault();

            AutoSelectTable firstTable = null;

            switch (SeatType)
            {
                case ActionTableTypes.Two:
                    firstTable = firstOneSeatTable ?? firstTwoSeatTable;
                    break;

                case ActionTableTypes.Four:
                    firstTable = firstFourSeatTable;
                    break;

                case ActionTableTypes.TwoAndFour:
                    firstTable = firstTwoSeatTable ?? firstFourSeatTable;
                    break;
            }

            return
                firstTable != null
                    ? firstTable.Seats.OrderBy(p => p.Number).Take(neededSeats).Select(p => new BusObject { Type = BusObjectTypes.Seat, Id = p.Id }).ToArray()
                    : new BusObject[0];
        }
    }
}
