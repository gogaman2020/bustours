using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class Client: BaseEntity
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsSigned { get; set; }
    }
}
