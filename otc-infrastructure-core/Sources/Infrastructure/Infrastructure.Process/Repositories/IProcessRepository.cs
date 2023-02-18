using System.Threading.Tasks;

namespace Infrastructure.Process.Repositories
{
    /// <summary>
    /// Интерфейс репозитария процесса
    /// </summary>
    public interface IProcessRepository<T>
    {
        /// <summary>
        /// Получает репозитарий процесса для состояния
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        ValueTask<IProcessStepRepository> UseProcessAsync(T state);
    }
}
