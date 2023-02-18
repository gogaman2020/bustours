using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Db.Responses
{
    /// <summary>
    /// Ответ на запрос
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class DataSourceResponse<T>
    {
        /// <summary>
        /// Коллекция элементов
        /// </summary>
        public T[] Items { get; set; }

        /// <summary>
        /// Общее количество
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Аггрегация
        /// </summary>
        public object Aggregates { get; set; }

        /// <summary>
        /// Группы
        /// </summary>
        public object Groups { get; set; }
    }
}
