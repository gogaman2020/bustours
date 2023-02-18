using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class SignUpCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly IUserRepository _userRepository;

        public SignUpCommand()
        {
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }

        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            var existingUser = await _userRepository.GetUserAsync(UserName);

            if (existingUser != null)
            {
                return Fail("Username already exists.");
            }

            var user = new User
            {
                UserName = UserName,
                Password = Password,
                Role = Role,
                Token = string.Empty
            };

            var hashed = GetHashedPassword(user.Password, out byte[] salt);
            user.Password = hashed;
            user.PasswordSalt = salt;

            var userId = await _userRepository.AddUserAsync(user);
            var addedUser = await _userRepository.GetUserAsync(userId);

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