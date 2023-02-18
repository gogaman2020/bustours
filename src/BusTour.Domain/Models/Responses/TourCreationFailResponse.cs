using System.Collections.Generic;
using System.Linq;
using DomainTour = BusTour.Domain.Entities.Tour;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Ошибка создания тура
    /// </summary>
    public class TourCreationFailResponse : BaseResponse
    { 
        /// <summary>
        /// Дублирующие туры
        /// </summary>
        public IEnumerable<DomainTour> DuplicateTours { get; set; }

        /// <summary>
        /// Блокирующие туры
        /// </summary>
        public IEnumerable<DomainTour> BlockingTours { get; set; }

        public bool HasDuplicateTours => DuplicateTours?.Any() == true;

        public bool HasBlockingTours => BlockingTours?.Any() == true;
    }
}
