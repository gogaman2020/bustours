using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.Notifications.Queries
{
    public class NotificationQuery : CrudQuery<Notification, NotificationQuery>
    {
        public static string Select(IEnumerable<string> fields) =>
            Getter.Get("SelectNotification", null, fields);
    }
}
