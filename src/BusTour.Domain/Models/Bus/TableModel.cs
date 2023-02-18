using BusTour.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Models.Bus
{
    /// <summary>
    /// Модель стола.
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Тип.
        /// </summary>
        public TableTypes Type { get; set; }

        /// <summary>
        /// Признак, что стол выбран.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Признак, что стол расположен в первом ряду.
        /// </summary>
        public bool IsFirstRow { get; set; }

        /// <summary>
        /// Признак, что стол расположен в последнем ряду.
        /// </summary>
        public bool IsLastRow { get; set; }

        /// <summary>
        /// Колллекция мест за столом.
        /// </summary>
        public List<SeatModel> Seats { get; set; }

        /// <summary>
        /// Признак, что стол свободен.
        /// </summary>
        public bool IsFree => !IsSelected && Seats.All(p => p.IsAvailable);

        /// <summary>
        /// Признак, что стол недоступен для выбора.
        /// </summary>
        public bool IsLocked => Seats.All(p => p.IsLocked);

        /// <summary>
        /// Признак, что стол доступен для выбора.
        /// </summary>
        public bool IsAvailable => !IsSelected && Seats.All(p => !p.IsLocked) && Seats.Any(p => p.IsAvailable);

        /// <summary>
        /// Количество доступных для выбора мест за столом.
        /// </summary>
        public int CountAvailableSeats => Seats.Count(p => p.IsAvailable);

        /// <summary>
        /// Количество выбранных мест за столом.
        /// </summary>
        public int CountSelectedSeats => Seats.Count(p => p.IsSelected) + (IsSelected ? Seats.Count : 0);
    }
}
