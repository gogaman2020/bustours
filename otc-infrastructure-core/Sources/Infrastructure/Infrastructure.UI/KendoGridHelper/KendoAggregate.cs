using System;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Аггрегации для Kendo.
    /// 
    /// The supported aggregates are "average", "count", "max", "min" and "sum".
    /// </summary>
    [Serializable]
    public class KendoAggregate
    {
        /// <summary>
        /// Количество.
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public object Count { get; set; }

        /// <summary>
        /// Сумма.
        /// </summary>
        [JsonProperty(PropertyName = "sum")]
        public object Sum { get; set; }

        /// <summary>
        /// Минимальные значение.
        /// </summary>
        [JsonProperty(PropertyName = "min")]
        public object Min { get; set; }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        [JsonProperty(PropertyName = "max")]
        public object Max { get; set; }

        /// <summary>
        /// Среднее значение.
        /// </summary>
        [JsonProperty(PropertyName = "average")]
        public object Average { get; set; }
    }
}