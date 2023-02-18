using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.Notifications
{
    public interface INotificationServiсe
    {
        /// <summary>
        /// Добавляем уведомление
        /// </summary>
        /// <param name="notificationEvent"></param>
        Task<Notification> AddNotificationAsync(INotificationEvent notificationEvent);

        /// <summary>
        /// Отправляем уведомление
        /// </summary>
        /// <param name="notification"></param>
        Task SendNotificationAsync(Notification notification);

        /// <summary>
        /// Получаем нотификации для отсылки
        /// </summary>
        /// <returns></returns>
        Task<List<Notification>> GetNotificationsToSendAsync();

        /// <summary>
        /// Отправляем письмо
        /// </summary>
        Task SendEmailAsync(string email, string subject, string body);
    }
}
