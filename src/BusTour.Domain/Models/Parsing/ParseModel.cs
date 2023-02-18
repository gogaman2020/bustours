using BusTour.Domain.Enums;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Parsing
{
    /// <summary>
    /// Модель для парсинга файла с правилами.
    /// </summary>
    public class ParseModel
    {
        /// <summary>
        /// Коллекция типов правил для столов на двоих.
        /// </summary>
        public List<RuleTypesTwoSeats> RuleTypesTwoSeats { get; set; }

        /// <summary>
        /// Коллекция типов правил для столов на четверых.
        /// </summary>
        public List<RuleTypesFourSeats> RuleTypesFourSeats { get; set; }

        /// <summary>
        /// Коллекция правил.
        /// </summary>
        public List<ParseRuleModel> Rules { get; set; }

        /// <summary>
        /// Отладочная информация.
        /// </summary>
        public ParseDebugModel Debug { get; set; }
    }
}
