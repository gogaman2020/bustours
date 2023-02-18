using System;
using System.Threading.Tasks;

namespace Infrastructure.Mediator.LockManager
{
    public interface ILockManager
    {
        Task<bool> WithLockAsync(string key, Func<Task> func);
    }
}
