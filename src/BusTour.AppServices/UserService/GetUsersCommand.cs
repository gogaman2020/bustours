using BusTour.Data.Repositories.Users;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class GetUsersCommand : HighLevelMediatorCommand<IEnumerable<UserViewModel>>
    {
        private readonly IUserRepository _userRepository;
        public GetUsersCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public override async Task<MediatorCommandResult<IEnumerable<UserViewModel>>> ExecuteAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var result = users
                .Select(p => p.ToViewModel())
                .ToList();

            return Success(result);
        }
    }
}