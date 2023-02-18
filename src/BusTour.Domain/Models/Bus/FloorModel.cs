using System.Collections.Generic;

namespace BusTour.Domain.Models.Bus
{
    /// <summary>
    /// Модель  этажа автобуса.
    /// </summary>
    public class FloorModel
    {
        /// <summary>
        /// Коллекция столов.
        /// </summary>
        public List<TableModel> Tables { get; set; }
    }
}
