using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;

namespace BusTour.Domain.Entities
{
    public class PromoCodeGridModel : BaseEntity
    {
        public int? NumberOfPromocodes { get; set; }

        public string SeriesNumber { get; set; }

        public TypeOfDiscount DiscountType { get; set; }

        public PromoCodeType PromoCodeType { get; set; }

        public decimal AmountOfDiscount { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string QuantityIssued => NumberOfPromocodes.HasValue ? NumberOfPromocodes.Value.ToString() : "quantity not limited";

        public int QuantityUsed { get; set; }

        public decimal? DiscountPlanned => DiscountType == TypeOfDiscount.ByPercent ? null : NumberOfPromocodes * AmountOfDiscount;

        public decimal? DiscountUsed => DiscountType == TypeOfDiscount.ByPercent ? (decimal?)null : QuantityUsed * AmountOfDiscount;

        public bool IsActive { get; set; }
    }
}
