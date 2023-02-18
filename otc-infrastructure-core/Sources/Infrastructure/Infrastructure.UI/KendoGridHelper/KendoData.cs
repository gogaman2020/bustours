using System;
using System.Collections.Generic;

namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Модель для DataSource.
    /// </summary>
    [Serializable]
    public class KendoData
    {
        /// <summary>
        /// Коллекция элементов.
        /// </summary>
        public object Items { get; set; }

        /// <summary>
        /// Общее количество элементов.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Группы, если поддерживается группировка.
        /// </summary>
        public List<KendoGroup> Groups { get; set; }

        /// <summary>
        /// Агрегации.
        /// </summary>
        public object Aggregates { get; set; }
    }
}