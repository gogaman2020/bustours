using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Тип промокода
    /// </summary>
    public enum PromoCodeType
    {
        /// <summary>
        /// Только временной
        /// </summary>
        ByDate = 1,

        /// <summary>
        /// Только количественный
        /// </summary>
        ByAmount = 2,

        /// <summary>
        /// Временной и количественный
        /// </summary>
        ByDateAndAmount = 3,
    }
}
