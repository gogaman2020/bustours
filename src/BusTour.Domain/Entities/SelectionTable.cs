using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Стол для выбора.
    /// </summary>
    public class SelectionTable : BaseEntity
    {
        /// <summary>
        /// Номер.
        /// </summary>
        public byte Number { get; set; }

        /// <summary>
        /// Этаж.
        /// </summary>
        public byte Floor { get; set; }

        /// <summary>
        /// Категория.
        /// </summary>
        public TableCategories TableCategory { get; set; } 

        /// <summary>
        /// Расположение.
        /// </summary>
        public TableLocations TableLocation { get; set; }

        /// <summary>
        /// Коллекция мест за столом.
        /// </summary>
        public List<SelectionSeat> Seats { get; set; }
    }
}
