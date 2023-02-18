using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Enums
{
    public enum TourType
    {
        /// <summary>
        /// Регулярный тур
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Частный тур
        /// </summary>
        PrivateHire = 20,

        /// <summary>
        /// Сервисный тур
        /// </summary>
        Service = 30,
    }
}
