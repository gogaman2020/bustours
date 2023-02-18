using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Notifications
{
    public interface INotificationRepository : ICrudRepository<Notification>
    {
        /// <summary>
        /// Получить уведомления по фильтру
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<Notification>> SelectAsync(NotificationFilter filter);
    }
}
