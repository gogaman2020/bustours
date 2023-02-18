using Newtonsoft.Json;

namespace Infrastructure.Db.Requests
{
    /// <summary>
    /// Сортировка
    /// </summary>
    public class DataSourceSort
    {
        /// <summary>
        /// Поле
        /// </summary>
        [JsonProperty("field")]
        public string Field { get; set; }

        /// <summary>
        /// Направление (asc, desc)
        /// </summary>
        [JsonProperty("dir")]
        public string Direction { get; set; }
    }
}
