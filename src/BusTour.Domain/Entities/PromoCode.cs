using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    public class PromoCode : BaseEntity
    {
        /// <summary>
        /// Вид Промокода
        /// </summary>
        public PromoCodeType PromoCodeType { get; set; }

        /// <summary>
        /// Серия номер промокода
        /// </summary>
        public string SeriesNumber { get; set; }

        /// <summary>
        /// Дата создания промокода
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Дата начала действия промокода
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// Дата окончания действия промокода
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Кол-во промокодов
        /// </summary>
        public int? NumberOfPromocodes { get; set; }

        /// <summary>
        /// Количество использований
        /// </summary>
        public int? NumberOfUses { get; set; }

        /// <summary>
        /// Размер скидки
        /// </summary>
        public decimal AmountOfDiscount { get;set; }

        /// <summary>
        /// Тип скидки
        /// </summary>
        public TypeOfDiscount TypeOfDiscount { get; set; }

        /// <summary>
        /// Id города
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Активный промокод
        /// </summary>
        public bool IsActive { get; set; }
    }
}
