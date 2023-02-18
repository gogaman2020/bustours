using BusTour.Common.Config;
using BusTour.Common.Services;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class GetCurrentUserCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        public GetCurrentUserCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            var context = IoC.GetRequiredService<IUserContext>();

            if (context.UserId == default(int))
            {
                return Fail("Unable to get current user");
            }

            var user = await _userRepository.GetUserAsync(context.UserId);

            return Success(user.ToViewModel());
        }
    }
}