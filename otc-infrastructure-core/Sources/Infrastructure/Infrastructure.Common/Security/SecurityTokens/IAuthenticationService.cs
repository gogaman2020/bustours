using System;
using System.Threading.Tasks;

namespace Infrastructure.Security.SecurityTokens
{
    public interface IAuthenticationService
    {
        Task RunAsSysUserAsync(string sysuser, int organizationId, int? platformId, Func<IServiceProvider,Task> func);
    }
}