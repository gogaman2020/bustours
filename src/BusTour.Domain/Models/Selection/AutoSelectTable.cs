using BusTour.Domain.Enums;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Selection
{
    /// <summary>
    /// Модель стола для автоматического выбора
    /// </summary>
    public class AutoSelectTable
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Тип.
        /// </summary>
        public TableTypes Type { get; set; }

        /// <summary>
        /// Признак. что стол расположен в первом ряду.
        /// </summary>
        public bool IsFirstRow { get; set; }

        /// <summary>
        /// Признак. что стол расположен в последнем ряду.
        /// </summary>
        public bool IsLastRow { get; set; }

        /// <summary>
        /// Коллекция мест за столом.
        /// </summary>
        public List<AutoSelectSeat> Seats { get; set; }
    }
}

