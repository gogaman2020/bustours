using System.ComponentModel.DataAnnotations;

namespace BusTour.Domain.Models.Auth
{
    public class AuthenticateModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}