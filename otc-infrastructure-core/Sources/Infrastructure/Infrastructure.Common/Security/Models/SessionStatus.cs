namespace Infrastructure.Security.Models
{
    /// <summary>
    /// Состояние сессии.
    /// </summary>
    public enum SessionStatus
    {
        /// <summary>
        /// Сессия активна.
        /// </summary>
        Active,

        /// <summary>
        /// Сессия скоро истечет.
        /// </summary>
        ExpiredSoon,

        /// <summary>
        /// Сессия истекла.
        /// </summary>
        Expired
    }
}
