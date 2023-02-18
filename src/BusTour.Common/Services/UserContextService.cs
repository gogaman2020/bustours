using Infrastructure.Common.DI;
using System.Linq;
using System.Security.Claims;

namespace BusTour.Common.Services
{
    [InjectAsScoped]
    public class UserContextService : IUserAuthSetter
    {
        public int UserId { get; private set; }

        public string Role { get; private set; }

        public void Set(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity?.IsAuthenticated == true)
            {
                var userId = claimsPrincipal.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
                if (int.TryParse(userId, out int id))
                {
                    UserId = id;
                }

                Role = claimsPrincipal.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            }
        }
    }
}
