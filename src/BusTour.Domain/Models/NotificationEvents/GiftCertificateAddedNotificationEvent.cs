using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.NotificationEvents
{
    public class GiftCertificateAddedNotificationEvent : INotificationEvent
    {
        public NotificationTemplateId TemplateId => NotificationTemplateId.GiftCertificate;

        private GiftCertificate _certificate;

        public int? ObjectId => _certificate?.Id;

        public Dictionary<string, object> GetTemplateData()
        {
            return new Dictionary<string, object>
            {
                { "Header", "PRIME Bus Tours" },
                { "Body", $"Congratulations! You have bought a gift certificate №{_certificate.Number}, Price: £{_certificate.AmountVariant.Amount}, Valid Until: {_certificate.DateEnd.ToString("dd.MM.yyyy")}" },
                {  "number", _certificate.Number },
                { "value",  $"£{_certificate.AmountVariant.Amount}" },
                { "valid until", _certificate.DateEnd.ToString("dd.MM.yyyy")}
            };
        }

        public string GetEmail()
        {
            return _certificate.Client?.Email;
        }

        public GiftCertificateAddedNotificationEvent(GiftCertificate certificate)
        {
            _certificate = certificate;
        }
    }
}
