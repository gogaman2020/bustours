using BusTour.Common.Services;

namespace BusTour.Test
{
    public class TestUserContext : IUserContext
    {
        public int UserId { get; set; }

        public string Role { get; set; }
    }
}
