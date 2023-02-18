using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Filters
{
    public class PaymentFilter
    {
        public int? OrderId { get; set; }
        public int? GiftCertificateId { get; set; }
    }
}
