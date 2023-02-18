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
    public class PostUserCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly IUserRepository _userRepository;
        public PostUserCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RetypePassword { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }

        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            if (UserName.Contains(" ") || Password.Contains(" ") || RetypePassword.Contains(" "))
            {
                return Fail("Fields cannot contain spaces");
            }

            if (await _userRepository.GetUserAsync(UserName) != null)
            {
                return Fail("User with this username is already registered");
            }

            if (!int.TryParse(Id, out int id))
            {
                id = 0;
            }

            if (Password != RetypePassword)
            {
                return Fail("That’s not the same password as the first one");
            }

            var user = new User
            {
                UserName = UserName,
                Password = GetHashedPassword(Password, out byte[] salt),
                PasswordSalt = salt,
                Role = Role,
                Token = Token,
                Id = id
            };

            await _userRepository.AddUserAsync(user);

            var addedUser = await _userRepository.GetUserAsync(user.Id);

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