using BusTour.AppServices.Notifications;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.NotificationEvents;
using Infrastructure.Common.DI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class NotificationsTest : UnitTestBase
    {
        [TestMethod]
        public async Task AddNotification()
        {
            var notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();

            var notificationEvent = new RegularOrderCreatedNotificationEvent(new Order 
            {
                Id = DateTime.Now.Minute,
                Client = new Client
                {
                    Email = "gogaman@mail.ru",
                    FullName = "Karabas Barabas"
                }
            });

            var notification = await notificationServiсe.AddNotificationAsync(notificationEvent);

            Assert.IsTrue(notification.Id != default);

            notificationEvent = new RegularOrderCreatedNotificationEvent(new Order
            {
                Id = DateTime.Now.Minute,
                Client = new Client
                {
                    Email = "gogaman2020@yandex.ru",
                    FullName = "Karabas Barabas"
                }
            });

            notification = await notificationServiсe.AddNotificationAsync(notificationEvent);

            Assert.IsTrue(notification.Id != default);
        }

        [TestMethod]
        public async Task SendNotifications()
        {
            var notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();

            await AddNotification();

            var notifications = await notificationServiсe.GetNotificationsToSendAsync();

            foreach(var notification in notifications)
            {
                await notificationServiсe.SendNotificationAsync(notification);
            }

            notifications = await notificationServiсe.GetNotificationsToSendAsync();

            Assert.AreEqual(0, notifications.Count);
        }
    }
}
