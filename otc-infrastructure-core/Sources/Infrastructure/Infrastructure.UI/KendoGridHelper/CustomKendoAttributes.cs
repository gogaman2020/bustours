using System;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Атрибуты.
    /// </summary>
    [Serializable]
    public class CustomKendoAttributes
    {
        /// <summary>
        /// Класс css.
        /// </summary>
        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }
    }
}