using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Шаблон уведомлений
    /// </summary>
    public class NotificationTemplate : BaseEntity
    {
        /// <summary>
        /// Тип шаблона
        /// </summary>
        public NotificationTemplateId TemplateType { get; set; }

        /// <summary>
        /// Заголовок письма
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Тело письмо
        /// </summary>
        public string Body { get; set; }
    }
}
