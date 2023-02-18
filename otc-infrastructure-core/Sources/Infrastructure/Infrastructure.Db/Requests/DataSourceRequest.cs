using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Db.Requests
{
    /// <summary>
    /// Запрос данных
    /// </summary>
    /// <typeparam name="TFilter">Тип фильтра</typeparam>
    public class DataSourceRequest<TFilter> : PagingQueryData
        where TFilter : class
    {
        /// <summary>
        /// Страница
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Фильтры
        /// </summary>
        public DataSourceFilter<TFilter> Filter { get; set; }

        /// <summary>
        /// Сортировки
        /// </summary>
        [JsonProperty("sort")] public DataSourceSort[] Sorts { get; set; }

        /// <summary>
        /// Получает первый объект фильтров.
        /// </summary>
        /// <returns></returns>
        public TFilter GetFilter()
        {
            return Filter?.Filters.FirstOrDefault();
        }

        /// <summary>
        /// Получает первый объект сортировки.
        /// </summary>
        /// <returns></returns>
        public DataSourceSort GetSort()
        {
            return Sorts?.FirstOrDefault();
        }

        /// <summary>
        /// Получает параметро пагинации и сортировки
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPagingAndOrder()
        {
            var sort = GetSort();

            //todo защиту от дб иньекций, строки из запроса пользователя прокидываются в поля напрямую.
            var dic = new Dictionary<string, string>
            {
                ["OrderFieldName"] = ResolveOrderField(sort?.Field),
                ["OrderDirection"] = sort?.Direction ?? "asc"
            };

            if (Skip.HasValue)
            {
                dic["Skip"] = Skip.Value.ToString();
            }

            if (Take.HasValue)
            {
                dic["Take"] = Take.Value.ToString();
            }

            return dic;
        }

        /// <summary>
        /// Разрешает имена полей в запросе для сортировки
        /// </summary>
        /// <param name="requiredField"></param>
        /// <returns></returns>
        protected virtual string ResolveOrderField(string requiredField)
        {
            return requiredField;
        }
    }
}
