using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Настройки колонок и агрегаций таблицы.
    /// </summary>
    [Serializable]
    public class KendoColumns
    {
        /// <summary>
        /// Список колонок.
        /// </summary>
        [JsonProperty(PropertyName = "columns")]
        public List<CustomKendoColumn> Columns { get; set; }

        /// <summary>
        /// Список агрегаций.
        /// </summary>
        [JsonProperty(PropertyName = "aggregates")]
        public List<CustomKendoAggregateConfig> Aggregates { get; set; }
    }
}