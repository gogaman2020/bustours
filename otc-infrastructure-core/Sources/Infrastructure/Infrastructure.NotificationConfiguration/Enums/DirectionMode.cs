using System.ComponentModel;

namespace Infrastructure.NotificationConfiguration.Enums
{
    /// <summary>
    /// Режим перевода статуса
    /// </summary>
    public enum DirectionMode : int
    {
        /// <summary>
        /// Нет
        /// </summary>
        [Description("Нет")]
        None = 0,

        /// <summary>
        /// Согласование
        /// </summary>
        [Description("Согласование")]
        Commit = 1,

        /// <summary>
        /// Отклонение
        /// </summary>
        [Description("Отклонение")]
        Reject = 2,

        /// <summary>
        /// Отмена
        /// </summary>
        [Description("Отмена")]
        Cancel = 3,

        /// <summary>
        /// Смена ответственных
        /// </summary>
        [Description("Смена ответственных")]
        ChangeResponsibles = 4
    }
}
