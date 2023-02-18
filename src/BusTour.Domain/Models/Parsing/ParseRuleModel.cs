using System.Text.Json.Serialization;
using BusTour.Domain.Enums;
using Newtonsoft.Json.Converters;

namespace BusTour.Domain.Models.Parsing
{
    /// <summary>
    /// Модель для парсинга правила.
    /// </summary>
    public class ParseRuleModel
    {
        /// <summary>
        /// Тип правила для столов на двоих.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleTypesTwoSeats RuleTypesTwoSeats { get; set; }

        /// <summary>
        /// Тип правила для столов на четверых.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleTypesFourSeats RuleTypesFourSeats { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - да, ответ на вопрос про столы на четверых - да.
        /// </summary>
        public ParseActionModel ActionYesYes { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - да, ответ на вопрос про столы на четверых - нет.
        /// </summary>
        public ParseActionModel ActionYesNo { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - нет, ответ на вопрос про столы на четверых - да.
        /// </summary>
        public ParseActionModel ActionNoYes { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - нет, ответ на вопрос про столы на четверых - нет.
        /// </summary>
        public ParseActionModel ActionNoNo { get; set; }
    }
}
