using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Типы конфликтов
    /// </summary>
    public enum OrdersConflictType
    {
        /// <summary>
        /// Место в автобусе
        /// </summary>
        Seat = 0,

        /// <summary>
        /// Весь автобус
        /// </summary>
        Bus = 10
    }
}
