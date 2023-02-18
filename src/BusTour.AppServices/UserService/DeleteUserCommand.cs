using BusTour.Common.Services;
using BusTour.Data.Repositories.Users;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class DeleteUserCommand : HighLevelMediatorCommand<int>
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public override async Task<MediatorCommandResult<int>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            var context = IoC.GetRequiredService<IUserContext>();

            if (context.UserId == id)
            {
                return Fail("Cannot delete yourself.");
            }

            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return Fail();
            }

            await _userRepository.DeleteUserAsync(id);

            return Success(id);
        }
    }
}