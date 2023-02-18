using System.ComponentModel;

namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Типы правил про столы на четверых.
    /// </summary>
    public enum RuleTypesFourSeats
    {
        /// <summary>
        /// Ещё свободный стол на четверых
        /// </summary>
        [Description("Ещё свободный стол на четверых")]
        MoreFreeTable,

        /// <summary>
        /// Свободный стол на четверых
        /// </summary>
        [Description("Свободный стол на четверых")]
        FreeTable,

        /// <summary>
        /// Три свободных места за столом на четверых
        /// </summary>
        [Description("Три свободных места за столом на четверых")]
        ThreeFreeSeats,

        /// <summary>
        /// Два свободных места за столом на четверых
        /// </summary>
        [Description("Два свободных места за столом на четверых")]
        TwoFreeSeats,

        /// <summary>
        /// Одно свободное место за столом на четверых
        /// </summary>
        [Description("Одно свободное место за столом на четверых")]
        OneFreeSeat,

        /// <summary>
        /// Два свободных стола в первом ряду или два свободных стола в последнем ряду или свободный стол на четверых
        /// </summary>
        [Description("Два свободных стола в первом ряду или два свободных стола в последнем ряду или свободный стол на четверых")]
        GroupTables
    }
}
