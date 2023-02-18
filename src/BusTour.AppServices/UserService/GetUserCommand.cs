using BusTour.Data.Repositories.Users;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class GetUserCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        public GetUserCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return Fail();
            }

            return Success(user.ToViewModel());
        }
    }
}