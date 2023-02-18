using System;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Конфигурация агрегаций.
    /// </summary>
    [Serializable]
    public class CustomKendoAggregateConfig
    {
        /// <summary>
        /// Свойтсво.
        /// </summary>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Агрегации.
        /// Одно значение: "sum"
        /// Cписок: ["sum", "count"]
        /// </summary>
        [JsonProperty(PropertyName = "aggregate")]
        public string Aggregate { get; set; }
    }
}