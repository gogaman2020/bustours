using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Уведолмение
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>
        /// Id объекта-источника
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Id шаблона
        /// </summary>
        public NotificationTemplateId TemplateId { get; set; }

        /// <summary>
        /// Шаблон
        /// </summary>
        [IgnoreField]
        public NotificationTemplate Template { get; set; }

        /// <summary>
        /// Email адресата
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Данные для отправки
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Дата последней отправки
        /// </summary>
        public DateTime? LastAttemptDate { get;set; }

        /// <summary>
        /// Признак успешной отправки
        /// </summary>
        public bool IsSent{ get; set; }

        public Notification()
        {

        }

        public Notification(INotificationEvent notificationEvent) : this()
        {
            Email = notificationEvent.GetEmail();
            Data = notificationEvent.GetTemplateData();
            TemplateId = notificationEvent.TemplateId;
            ObjectId = notificationEvent.ObjectId;
        }
    }

    public interface INotificationEvent
    {
        /// <summary>
        /// Получить email получаетеля уведомления
        /// </summary>
        /// <returns></returns>
        string GetEmail();

        /// <summary>
        /// Получить Id объекта-источника
        /// </summary>
        int? ObjectId { get; }

        /// <summary>
        /// Получить данные для шаблона
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetTemplateData();

        /// <summary>
        /// Тип события
        /// </summary>
        NotificationTemplateId TemplateId { get; }
    }
}
