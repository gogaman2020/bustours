using System.Security.Claims;

namespace BusTour.Common.Services
{
    public interface IUserAuthSetter: IUserContext
    {
        void Set(ClaimsPrincipal claimsPrincipal);
    }
}
