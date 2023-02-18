namespace Infrastructure.Db.Requests
{
    /// <summary>
    /// Паджинация
    /// </summary>
    public class PagingQueryData
    {
        /// <summary>
        /// Кол-во в выборке
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Кол-во пропущенных
        /// </summary>
        public int? Skip { get; set; }
    }
}
