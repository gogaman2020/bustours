using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Enums
{
    public enum GiftCertificateStatus
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Активен
        /// </summary>
        Active = 10,

        /// <summary>
        /// Просрочен
        /// </summary>
        Expired = 20,

        /// <summary>
        /// Использован
        /// </summary>
        Redeemed = 30,

        /// <summary>
        /// Аннулирован
        /// </summary>
        Сancelled = 99,
    }
}
