using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class TourBeverage: BaseEntity
    {
        public int TourId { get; set; }
        public int BeverageId { get; set; }
        public bool IsTicket { get; set; }
        public bool IsExtra { get; set; }
    }
}
