using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class GiftCertificate : BaseEntity
    {
        /// <summary>
        /// Номер сертификата
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окночания действия
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Вариант цены (id)
        /// </summary>
        public int? AmountVariantId { get; set; }

        /// <summary>
        /// Сертификат оплачен
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Сертификат отменен
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Id клиента
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Вариант цены
        /// </summary>
        [IgnoreField]
        public GiftCertificateAmountVariant AmountVariant { get; set; }

        /// <summary>
        /// Сумма использования
        /// </summary>
        public decimal? RedeemedAmount { get; set; }

        /// <summary>
        /// Дата использования
        /// </summary>
        public DateTime? RedeemedDate { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        [IgnoreField]
        public decimal? Amount => AmountVariant?.Amount;

        /// <summary>
        /// Статус
        /// </summary>
        [IgnoreField]
        public GiftCertificateStatus Status
        {
            get
            {
                if (Cancelled)
                {
                    return GiftCertificateStatus.Сancelled;
                }
                else if (RedeemedDate.HasValue)
                {
                    return GiftCertificateStatus.Redeemed;
                }
                else if (DateTime.UtcNow > DateEnd)
                {
                    return GiftCertificateStatus.Expired;
                }
                else if (IsPaid)
                {
                    return GiftCertificateStatus.Active;
                }
                else
                {
                    return GiftCertificateStatus.Draft;
                }
            }
        }

        [IgnoreField]
        public decimal Balance => (Amount ?? 0) - (RedeemedAmount ?? 0);

        /// <summary>
        /// Сюрпризы
        /// </summary>
        [IgnoreField]
        public List<GiftCertificateSurprise> CertificateSurprises { get; set; }

        /// <summary>
        /// Платеж
        /// </summary>
        [IgnoreField]
        public Payment Payment { get; set; }

        /// <summary>
        /// Заказ
        /// </summary>
        [IgnoreField]
        public Order Order { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [IgnoreField]
        public Client Client { get; set; }

        public GiftCertificate()
        {
            CertificateSurprises = new List<GiftCertificateSurprise>();
        }
    }
}
