using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class OrderSurprise: BaseEntity
    {
        public int OrderId { get; set; }
        public byte SurpriseId { get; set; }
        public byte Amount { get; set; }
    }
}
