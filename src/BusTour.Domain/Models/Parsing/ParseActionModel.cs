using System.Text.Json.Serialization;
using BusTour.Domain.Enums;
using Newtonsoft.Json.Converters;

namespace BusTour.Domain.Models.Parsing
{
    /// <summary>
    /// Модель для парсинга действия.
    /// </summary>
    public class ParseActionModel
    {
        /// <summary>
        /// Тип действия.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionTypes Type { get; set; }

        /// <summary>
        /// Переход на тип правила для столов на двоих.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleTypesTwoSeats? RedirectRuleTypesTwoSeats { get; set; }

        /// <summary>
        /// Переход на тип правила для столов на четверых.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleTypesFourSeats? RedirectRuleTypesFourSeats { get; set; }
    }
}
