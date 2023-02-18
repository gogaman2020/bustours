using BusTour.Common.Config;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.UserService
{
    [InjectAsScoped]
    public class AuthenticateUserCommand : HighLevelMediatorCommand<UserViewModel>
    {
        private readonly AuthorizationConfig _authConfig;
        private readonly IUserRepository _userRepository;

        public AuthenticateUserCommand()
        {
            _authConfig = Config.Get<AuthorizationConfig>();
            _userRepository = IoC.GetRequiredService<IUserRepository>();
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }

        public override async Task<MediatorCommandResult<UserViewModel>> ExecuteAsync()
        {
            var user = await _userRepository.GetUserAsync(UserName);

            if (user == null || string.Compare(UserName, user.UserName) != 0)
            {
                return Fail("Username is incorrect");
            }

            var userWToken = await AuthenticateAsync(user, Password);
            if (userWToken == null)
            {
                return Fail("Password is incorrect");
            }

            return Success(userWToken.ToViewModel());
        }

        private Task<User> AuthenticateAsync(User user, string password)
        {
            // check password
            if (GetHashedPassword(password, user.PasswordSalt) != user.Password)
            {
                return Task.FromResult((User)null);
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_authConfig.Expiration[user.Role]),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return Task.FromResult(user.WithoutPassword());
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