using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Infrastructure.NotificationConfiguration.Enums
{
    /// <summary>
    /// Движение статуса
    /// </summary>
    public enum StateDirection : int
    {
        /// <summary>
        /// Получение статуса
        /// </summary>
        [Description("Получение статуса")]
        In = 1,

        /// <summary>
        /// Уход со статуса
        /// </summary>
        [Description("Уход со статуса")]
        Out = 2,

        /// <summary>
        /// Переход в терминальное состояние
        /// </summary>
        [Description("Переход в терминальное состояние")]
        Terminate = 3
    }
}
