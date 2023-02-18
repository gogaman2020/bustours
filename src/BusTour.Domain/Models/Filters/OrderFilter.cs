using BusTour.Domain.Enums;
using BusTour.Domain.Helpers;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Filters
{
    public class OrderFilter
    {
        public IEnumerable<int> TourIds { get; set; }

        /// <remarks>В скриптах для поиска по статусу сравнивать это поле с tourorderstate.currentstepname</remarks>
        public IEnumerable<string> OrderProcessStates => ProcessHelper.GetStepNamesByEnumItems(States);

        public IEnumerable<OrderState> States { get; set; }

        public IEnumerable<string> Hashes { get; set; }
    }
}
