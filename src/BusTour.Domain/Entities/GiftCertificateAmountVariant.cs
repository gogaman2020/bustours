using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class GiftCertificateAmountVariant: BaseEntity
    {
        public decimal Amount { get; set; }
    }
}
