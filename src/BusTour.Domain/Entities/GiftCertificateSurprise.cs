using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class GiftCertificateSurprise: BaseEntity
    {
        public int CertificateId { get; set; }
        public int SurpriseId { get; set; }
        public int Quantity { get; set; }
    }
}
