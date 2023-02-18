using BusTour.AppServices.UserService;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [Authorize(Roles = Role.Supervisor + "," + Role.Administrator)]
    [InjectAsSingleton]
    public class UserController : BusTourControllerBase
    {
        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers()
        {
            return await RunCommandAsync(new GetUsersCommand());
        }

        [AllowAnonymous]
        [HttpGet("current")]
        public async Task<ActionResult<UserViewModel>> GetCurrentUser()
        {
            return await RunCommandAsync(new GetCurrentUserCommand());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(int id)
        {
            return await RunCommandAsync(new GetUserCommand { Id = id.ToString() });
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserViewModel>> PutUser(UpdateUserCommand command)
        {
            return await RunCommandAsync(command);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserViewModel>> PostUser(PostUserCommand user)
        {
            return await RunCommandAsync(user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return await RunCommandNoTypedResultAsync(new DeleteUserCommand { Id = id.ToString() });
        }
    }
}