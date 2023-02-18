using System.Threading.Tasks;

namespace Infrastructure.Common.Context
{
    /// <summary>
    /// Интерфейс использования контекстного объекта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectContext <T>
    {
        /// <summary>
        /// Объект контекста
        /// </summary>
        T Context { get; }

        /// <summary>
        /// Установка объекта по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task SetContextAsync(int id);

        /// <summary>
        /// Установка объекта
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task SetContextAsync(T t);
    }
}
