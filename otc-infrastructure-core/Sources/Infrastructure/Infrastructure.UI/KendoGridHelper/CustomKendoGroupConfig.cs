using System;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Настройка группировки.
    /// </summary>
    [Serializable]
    public class CustomKendoGroupConfig
    {
        /// <summary>
        /// Свойство.
        /// </summary>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }
    }
}