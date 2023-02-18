using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Группа Kendo.
    /// Модель группированных данных при серверной группировке Kendo.
    /// </summary>
    [Serializable]
    public class KendoGroup
    {
        /// <summary>
        /// Агрегации.
        /// Структура: 
        /// { 
        ///     aggregation-field-name: {
        ///         function-name: functions-value 
        ///     }
        /// }
        /// Функции агрегации лежат тут: <see cref="KendoAggregate"/>
        /// </summary>
        [JsonProperty(PropertyName = "aggregates")]
        public object Aggregates { get; set; }

        /// <summary>
        /// Свойство.
        /// </summary>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Значение.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        /// <summary>
        /// Коллекция группированных объектов.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public List<dynamic> Items { get; set; }

        /// <summary>
        /// Флаг вложенных групп.
        /// </summary>
        [JsonProperty(PropertyName = "hasSubgroups")]
        public bool HasSubgroups { get; set; }
    }
}