using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class User: BaseEntity
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }
    }
}