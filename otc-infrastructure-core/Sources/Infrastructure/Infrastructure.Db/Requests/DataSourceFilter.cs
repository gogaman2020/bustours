namespace Infrastructure.Db.Requests
{
    /// <summary>
    /// Фильтр запроса
    /// </summary>
    /// <typeparam name="TFilter">Тип фильтра</typeparam>
    public class DataSourceFilter<TFilter>
    {
        /// <summary>
        /// Логика (and, or)
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// Список фильтров
        /// </summary>
        public TFilter[] Filters { get; set; }
    }
}
