using System.ComponentModel;

namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Типы правил для столов на двоих.
    /// </summary>
    public enum RuleTypesTwoSeats
    {
        /// <summary>
        /// Фиктивное правило - всегда выполняется
        /// </summary>
        [Description("")]
        AlwaysTrue,

        /// <summary>
        /// Стол на двоих в первом ряду
        /// </summary>
        [Description("Стол на двоих в первом ряду")]
        TableFirstRow,

        /// <summary>
        /// Ещё свободный стол на двоих
        /// </summary>
        [Description("Ещё свободный стол на двоих")]
        MoreFreeTable,

        /// <summary>
        /// Свободный стол на двоих
        /// </summary>
        [Description("Свободный стол на двоих")]
        FreeTable,

        /// <summary>
        /// Одно место за столом на двоих
        /// </summary>
        [Description("Одно место за столом на двоих")]
        OneFreeSeat
    }
}
