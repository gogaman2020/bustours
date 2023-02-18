using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class TourMenu: BaseEntity
    {
        public int TourId { get; set; }
        public int MenuId { get; set; }
        public bool IsTicket { get; set; }
        public bool IsExtra { get; set; }
    }
}
