using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class OrderMenu: BaseEntity
    {
        public int? TourId { get; set; }
        
        public int OrderId { get; set; }

        public int? MenuId { get; set; }

        public int Amount { get; set; }

        public bool? Issued { get; set; }

    }
}
