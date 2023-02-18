using BusTour.Domain.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Модель автобуса для заказа.
    /// </summary>
    public class OrderBusModel
    {
        /// <summary>
        /// Коллекция столов.
        /// </summary>
        public List<OrderBusTableModel> Tables { get; set; }

        /// <summary>
        /// Правила для столов.
        /// </summary>
        public Dictionary<byte, List<dynamic>> TablesRules { get; set; }

        /// <summary>
        /// Правила для сидений.
        /// </summary>
        public Dictionary<string, List<dynamic>> SeatsRules { get; set; }

        [JsonIgnore]
        public List<OrderBusSeatModel> Seats 
        { 
            get
            {
                return Tables.SelectMany(x => x.Seats).ToList();
            }
        }
    }
}
