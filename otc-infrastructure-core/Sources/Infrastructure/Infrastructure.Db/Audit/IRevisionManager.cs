using System.Threading.Tasks;

namespace Infrastructure.Db.Audit
{
    /// <summary>
    /// Менеджер ревизий операций изменения сущностей.
    /// </summary>
    public interface IRevisionManager
    {
        /// <summary>
        /// Идентификатор текущего пользователя.
        /// </summary>
        int? CurrentUserId { get; }

        /// <summary>
        /// Получение номера текущей ревизии.
        /// </summary>
        /// <returns>Номер текущей ревизии.</returns>
        Task<int> GetCurrentRevisionNumber();
    }
}