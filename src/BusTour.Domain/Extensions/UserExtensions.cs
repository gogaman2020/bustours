using BusTour.Domain.Entities;
using BusTour.Domain.Models.Auth;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Extensions
{
    public static class UserExtensions
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            if (users == null) return null;

            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            if (user == null) return null;

            user.Password = null;
            return user;
        }

        public static UserViewModel ToViewModel(this User user)
        {
            return new UserViewModel { Id = user.Id, UserName = user.UserName, Role = user.Role, Token = user.Token };
        }
    }
}