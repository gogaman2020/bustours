using BusTour.Data.Repositories.Notifications.Queries;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Notifications
{
    [InjectAsSingleton]
    public class NotificationRepository : CrudRepository<Notification, NotificationQuery>, INotificationRepository
    {
        public async Task<List<Notification>> SelectAsync(NotificationFilter filter)
        {
            var notifications = new List<Notification>();

            await _db.ExecuteAsync(async (commands, ct) =>
            {
                notifications = (await commands.ExecuteFuncAsync(SelectInnerAsync, FilterQueryObject.For(filter, NotificationQuery.Select, true))).ToList();
            });

            return notifications;
        }

        public override async Task<Notification> GetAsync(int id)
        {
            return (await SelectAsync(new NotificationFilter { Id = id })).FirstOrDefault();
        }

        private Task<IEnumerable<Notification>> SelectInnerAsync(IDbConnection connection, string sql, object param, IDbTransaction transaction, int? timeout)
        {
            return connection
                .QueryAsync<Notification, NotificationTemplate, Notification>(
                    sql,
                    (notification, notificationTemplate) =>
                    {
                        notification.Template = notificationTemplate;
                        return notification;
                    },
                    param,
                    transaction,
                    commandTimeout: timeout
                );
        }
    }
}
