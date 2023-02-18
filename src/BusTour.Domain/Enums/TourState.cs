namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Статусы тура
    /// </summary>
    public enum TourState
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 10,

        /// <summary>
        /// Активный
        /// </summary>
        Active = 20,

        /// <summary>
        /// Запрос на отмену
        /// </summary>
        CancelRequest = 30,

        /// <summary>
        /// Отменен
        /// </summary>
        Canceled = 40,

        /// <summary>
        /// Удален
        /// </summary>
        Deleted = 99,
    }
}
