using BusTour.Data.Repositories.Notifications;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Filters;
using Infrastructure.Common.DI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BusTour.Common.Config;
using Infrastructure.Common.Configs;
using NLog;
using Newtonsoft.Json;
using System.IO;
using BusTour.AppServices.FileConvertingService;
using System.Linq;
using Infrastructure.Mediator;
using BusTour.AppServices.GiftCertificates.Queries;

namespace BusTour.AppServices.Notifications
{
    [InjectAsSingleton]
    public class NotificationServiсe : INotificationServiсe
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IFileConvertingService _fileConvertingService;
        private readonly SmtpConfig _smtpConfig;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public NotificationServiсe(INotificationRepository notificationRepository, IFileConvertingService fileConvertingService, IMediator mediator)
        {
            _notificationRepository = notificationRepository;
            _fileConvertingService = fileConvertingService;
            _smtpConfig = Config.Get<SmtpConfig>();
            _logger = LogManager.GetCurrentClassLogger();
            _mediator = mediator;
        }

        public async Task<Notification> AddNotificationAsync(INotificationEvent notificationEvent)
        {
            //return new Notification();
            var id = await _notificationRepository.SaveOrUpdateAsync(new Notification(notificationEvent));
            return await _notificationRepository.GetAsync(id);
        }

        public Task<List<Notification>> GetNotificationsToSendAsync()
        {
            return _notificationRepository.SelectAsync(new NotificationFilter { IsSent = false, HasEmail = true });
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            //return;
            if (!string.IsNullOrEmpty(notification.Email))
            {
                try
                {
                    using (var smtp = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port))
                    {

                        if (!string.IsNullOrEmpty(_smtpConfig.User) && !string.IsNullOrEmpty(_smtpConfig.Password))
                        {
                            smtp.Credentials = new NetworkCredential(_smtpConfig.User, _smtpConfig.Password);
                        }
                        else
                        {
                            smtp.UseDefaultCredentials = true;
                        }

                        var message = new MailMessage(
                           _smtpConfig.From,
                            notification.Email,
                            notification.Template.Subject,
                            GetBodyHtml(notification)
                        )
                        {
                            IsBodyHtml = true
                        };

                        if (notification.Template.Id == 40)
                        {
                            _logger.Error("Start generate pdf");
                            var pdf = (await _mediator.RunCommandAsync(new GetCertificatePdfCommand(notification.Data["number"].ToString()))).Result;
                            _logger.Error("Finish generate pdf");

                            using var pdfStream = new MemoryStream(pdf);
                            using var attachment = new Attachment(pdfStream, "gift_certificate.pdf");
                            message.Attachments.Add(attachment);
                            smtp.Send(message);
                        }
                        else
                        {
                            smtp.Send(message);
                        }

                        notification.IsSent = true;
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error($"Sending notification: {notification.Id}");
                    _logger.Error(exception);
                }

                notification.LastAttemptDate = DateTime.UtcNow;

                var id = await _notificationRepository.SaveOrUpdateAsync(notification);
            }

            _logger.Error("Method end");
        }

        private string GetBodyHtml(Notification notification)
        {
            var bodyTemplate = notification.Template.Body;

            foreach(var kvp in notification.Data)
            {
                bodyTemplate = bodyTemplate.Replace("{" + kvp.Key + "}", kvp.Value.ToString());
            }

            return bodyTemplate;
        }

        public Task SendEmailAsync(string email, string subject, string body)
        {
            if (!string.IsNullOrEmpty(email))
            {
                using (var smtp = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port))
                {

                    if (!string.IsNullOrEmpty(_smtpConfig.User) && !string.IsNullOrEmpty(_smtpConfig.Password))
                    {
                        smtp.Credentials = new NetworkCredential(_smtpConfig.User, _smtpConfig.Password);
                    }
                    else
                    {
                        smtp.UseDefaultCredentials = true;
                    }

                    var message = new MailMessage(
                        _smtpConfig.From,
                        email,
                        subject,
                        body
                    )
                    {
                        IsBodyHtml = true
                    };

                    smtp.Send(message);
                }
            }
            else
                throw new ApplicationException("Email address is empty");

            return Task.CompletedTask;
        }
    }
}
