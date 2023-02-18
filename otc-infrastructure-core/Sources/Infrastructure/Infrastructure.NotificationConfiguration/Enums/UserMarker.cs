using System.ComponentModel;

namespace Infrastructure.NotificationConfiguration.Enums
{
    /// <summary>
    /// Маркеры пользователей
    /// </summary>
    public enum UserMarker : int
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        [Description("Пользователь")]
        User = 1,
        
        /// <summary>
        /// Рабочая группа
        /// </summary>
        [Description("Рабочая группа")]
        WorkGroup = 2,

        /// <summary>
        /// Заявитель
        /// </summary>
        [Description("Заявитель")]
        Organizer = 3,

        /// <summary>
        /// Инициатор
        /// </summary>
        [Description("Инициатор")]
        Initiator = 4,

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Description("Исполнитель")]
        Executor = 5,

        /// <summary>
        /// Ответственный
        /// </summary>
        [Description("Ответственный")]
        Responsible = 6,
    }
}
