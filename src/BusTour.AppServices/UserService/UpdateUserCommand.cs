using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class UpdateUserCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }


        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            if ((UserName != null && UserName.Contains(" ")) || (Password != null && Password.Contains(" ")))
            {
                return Fail("Fields cannot contain spaces");
            }

            if (int.TryParse(Id, out var id) == false || id < 1)
            {
                return Fail();
            }

            var userById = await _userRepository.GetUserAsync(id);
            var userByName = await _userRepository.GetUserAsync(UserName);

            if (userByName != null && userByName.Id != id)
            {
                return Fail("User with this username is already registered");
            }

            userById.UserName = UserName;
            userById.Role = Role;

            if (Password != null)
            {
                userById.Password = GetHashedPassword(Password, out byte[] salt);
                userById.PasswordSalt = salt;
            }

            await _userRepository.UpdateUserAsync(userById);

            var addedUser = await _userRepository.GetUserAsync(userById.Id);

            return Success(addedUser.ToViewModel());
        }

        private string GetHashedPassword(string password, out byte[] salt)
        {
            // generate a 128-bit salt using a secure PRNG
            salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return GetHashedPassword(password, salt);
        }

        private string GetHashedPassword(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}