using BusTour.Data.Repositories.Users.Queries;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Users
{
    [InjectAsSingleton]
    public class UserRepository : CrudRepository<User, GetUserQuery>, IUserRepository
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(UserRepository).Name);
        public async Task<User> GetUserAsync(int id)
        {
            try
            {
                return await GetAsync(id);
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.GetUser threw error");
                throw;
            }
        }

        public async Task<User> GetUserAsync(string userName)
        {
            try
            {
                var user = await _db.QueryFirstOrDefaultAsync<User>(FilterQueryObject.For(new GetUserQuery { UserName = userName }, GetUserQuery.SelectByFilter));
                return user;
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.GetUser threw error");
                throw;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var users = await _db.QueryAsync<User>(FilterQueryObject.For(new GetUserQuery(), GetUserQuery.SelectByFilter));
                return users.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.GetUsers threw error");
                throw;
            }
        }

        public async Task<int> AddUserAsync(User user)
        {
            try
            {
                return await SaveOrUpdateAsync(user);
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.AddUser threw error");
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                await SaveOrUpdateAsync(user);
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.UpdateUser threw error");
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                await DeleteAsync(new User { Id = id });
            }
            catch (Exception e)
            {
                _logger.Error(e, "UserRepository.DeleteUser threw error");
                throw;
            }
        }
    }
}