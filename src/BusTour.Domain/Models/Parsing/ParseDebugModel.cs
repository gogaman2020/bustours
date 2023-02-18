using BusTour.Domain.Enums;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Parsing
{
    /// <summary>
    /// Модель для парсинга отладочной информации.
    /// </summary>
    public class ParseDebugModel
    { 
        /// <summary>
        /// Код файла с правилами.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Наименование коллекции правил.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание типов правил для столов на двоих.
        /// </summary>
        public Dictionary<RuleTypesTwoSeats, ParseDebugItem> RuleTypesTwoSeats { get; set; }

        /// <summary>
        /// Описание типов правил для столов на четверых.
        /// </summary>
        public Dictionary<RuleTypesFourSeats, ParseDebugItem> RuleTypesFourSeats { get; set; }

        /// <summary>
        /// Описание действий.
        /// </summary>
        public List<ParseDebugActions> Actions { get; set; }
    }

    /// <summary>
    /// Объект для отображения отладочной информации.
    /// </summary>
    public class ParseDebugItem
    { 
        /// <summary>
        /// Текст.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Цвет текста.
        /// </summary>
        public string Color { get; set; }
        
        /// <summary>
        /// Цвет фона.
        /// </summary>
        public string Background { get; set; }
    }

    /// <summary>
    /// Модель для парсинга отладочной информации по правилам.
    /// </summary>
    public class ParseDebugActions : ParseDebugItem
    {
        /// <summary>
        /// Коллекция подходящих типов правил.
        /// </summary>
        public List<ActionTypes> Types { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description { get; set; }
    }
}
