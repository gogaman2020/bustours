using BusTour.AppServices.UserService;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [InjectAsSingleton]
    public class AuthController : BusTourControllerBase
    {
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<UserViewModel>> Authenticate([FromBody] AuthenticateUserCommand model)
        {
            return await RunCommandAsync(model);
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<UserViewModel>> Signup(SignUpCommand model)
        {
            return await RunCommandAsync(model);
        }
    }
}