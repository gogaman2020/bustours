using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    public class Payment : BaseEntity
    {
        /// <summary>
        /// Оплачиваемый заказ (id)
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// Оплачиваемый сертификат (id)
        /// </summary>
        public int? GiftCertificateId { get; set; }

        /// <summary>
        /// Внешний идентификатор оплаты
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Результат оплаты
        /// </summary>
        [AsIs]
        public PaymentDetails Details { get; set; }

        public Payment()
        {
            this.Details = new PaymentDetails();
        }
    }

    /// <summary>
    /// Детали оплаты
    /// </summary>
    public class PaymentDetails
    {
        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string Error { get; set; }
    }
}
