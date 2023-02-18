using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Настройка колонки.
    /// </summary>
    [Serializable]
    public class CustomKendoColumn
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CustomKendoColumn()
        {
            FooterAttributes = new CustomKendoAttributes { Class = "text-center footer-total" };
            HeaderAttributes = new CustomKendoAttributes { Class = "text-center" };
            Attributes = new CustomKendoAttributes { Class = "text-center" };
            Sortable = false;
        }

        /// <summary>
        /// Свойство.
        /// </summary>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Ширина.
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public string Width { get; set; }

        /// <summary>
        /// Шаблон заголовка группы.
        /// </summary>
        [JsonProperty(PropertyName = "groupHeaderTemplate")]
        public string GroupHeaderTemplate { get; set; }

        /// <summary>
        /// Шаблон нижнего колонтитула.
        /// </summary>
        [JsonProperty(PropertyName = "footerTemplate")]
        public string FooterTemplate { get; set; }

        /// <summary>
        /// Шаблон нижнего колонтитула группы.
        /// </summary>
        [JsonProperty(PropertyName = "groupFooterTemplate")]
        public string GroupFooterTemplate { get; set; }

        /// <summary>
        /// Сортируемая.
        /// </summary>
        [JsonProperty(PropertyName = "sortable")]
        public bool? Sortable { get; set; }

        /// <summary>
        /// Скрытая.
        /// </summary>
        [JsonProperty(PropertyName = "hidden")]
        public bool? Hidden { get; set; }

        /// <summary>
        /// Freeze.
        /// </summary>
        [JsonProperty(PropertyName = "locked")]
        public bool? Locked { get; set; }
        
        /// <summary>
        /// Атрибуты заголовка.
        /// </summary>
        [JsonProperty(PropertyName = "headerAttributes")]
        public CustomKendoAttributes HeaderAttributes { get; set; }

        /// <summary>
        /// Атрибуты значения.
        /// </summary>
        [JsonProperty(PropertyName = "attributes")]
        public CustomKendoAttributes Attributes { get; set; }

        /// <summary>
        /// Атрибуты нижнего колонтитула.
        /// </summary>
        [JsonProperty(PropertyName = "footerAttributes")]
        public CustomKendoAttributes FooterAttributes { get; set; }

        /// <summary>
        /// Дочерние колонки.
        /// </summary>
        [JsonProperty(PropertyName = "columns")]
        public List<CustomKendoColumn> Columns { get; set; }
    }
}