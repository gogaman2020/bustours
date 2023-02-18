using System.Threading.Tasks;

namespace Infrastructure.Security.SecurityTokens
{
    public interface IAuthClient
    {
        Task<SecurityTokenClientResponse> SystemUserAuthenticateAsync(string sysusername, int organizationId, int? platformId = null);
    }
}