using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class OrderBeverage: BaseEntity
    {
        public int OrderId { get; set; }

        public int BeverageId { get; set; }
        
        public int? TourId { get; set; }

        public int Amount { get; set; }

        public bool? Issued { get; set; }
    }
}
