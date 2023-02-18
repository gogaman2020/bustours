using BusTour.Domain.Models.Bus;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Selection
{
    /// <summary>
    /// Результат подбора мест.
    /// </summary>
    public class SelectionResult
    { 
        /// <summary>
        /// Список объектов, доступных для выбора.
        /// </summary>
        public List<BusObject> AvailableObjects { get; set; }
    }
}

