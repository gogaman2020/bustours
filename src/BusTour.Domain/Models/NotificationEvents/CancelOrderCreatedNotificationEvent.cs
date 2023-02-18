using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.NotificationEvents
{
    public class CancelOrderCreatedNotificationEvent : INotificationEvent
    {
        public NotificationTemplateId TemplateId => NotificationTemplateId.CancelOrder;

        private DomainOrder _order;

        public int? ObjectId => _order?.Id;

        public Dictionary<string, object> GetTemplateData()
        {
            return new Dictionary<string, object>
            {
                 { "Customer", _order.Client?.FullName ?? "Customer" },
                 { "BookingNumber", _order.Id.ToString() },
                 { "BookingDate", _order.Tour.Departure.ToString("dd.MM.yyyy HH:mm") }
            };
        }

        public string GetEmail()
        {
            return _order.Client.Email;
        }

        public CancelOrderCreatedNotificationEvent(DomainOrder order)
        {
            _order = order;
        }
    }
}
