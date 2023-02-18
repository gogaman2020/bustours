using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о столе.
    /// </summary>
    public class ResponseTable
    { 
        /// <summary>
        /// ИД стола.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Признак - стол заблокирован.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Признак - стол выбран.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Признак - стол доступен для выбора.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Коллекция мест за столом.
        /// </summary>
        public List<ResponseSeat> Seats { get; set; }
    }
}
