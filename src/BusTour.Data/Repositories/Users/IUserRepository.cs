using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Users
{
    public interface IUserRepository: ICrudRepository<User>
    {
        Task<User> GetUserAsync(int id);

        Task<User> GetUserAsync(string userName);

        Task<List<User>> GetUsersAsync();

        Task<int> AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(int id);
    }
}