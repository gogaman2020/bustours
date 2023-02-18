using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.NotificationEvents
{
    public class PrivateHireOrderCreatedNotificationEvent : INotificationEvent
    {
        public NotificationTemplateId TemplateId => NotificationTemplateId.PrivateHireOrder;

        private DomainOrder _order;

        public int? ObjectId => _order?.Id;

        public Dictionary<string, object> GetTemplateData()
        {
            return new Dictionary<string, object>
            {
                 { "Id", _order.Id.ToString() }
            };
        }

        public string GetEmail()
        {
            return _order.Client.Email;
        }

        public PrivateHireOrderCreatedNotificationEvent(DomainOrder order)
        {
            _order = order;
        }
    }
}
