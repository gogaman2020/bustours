using BusTour.AppServices.Notifications;
using BusTour.AppServices.TourProcess;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Auth;
using BusTour.Domain.Models.NotificationEvents;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BusTour.AppServices.GiftCertificates.Commands
{
    [InjectAsScoped]
    public class GiftCertificateAddCommand : HighLevelMediatorCommand<GiftCertificate>
    {
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Вариант суммы (id)
        /// </summary>
        public int? AmountVariantId { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Есть ли сюрпризы
        /// </summary>
        public bool HasSurprises { get; set; }

        /// <summary>
        /// Сюрпризы
        /// </summary>
        public List<GiftCertificateSurprise> CertificateSurprises { get; set; }

        /// <summary>
        /// Email клиента
        /// </summary>
        public string Email { get; set; }

        private readonly IGiftCertificateRepository _giftCertificateRepository;
        private readonly INotificationServiсe _notificationServiсe;


        public GiftCertificateAddCommand()
        {
            _giftCertificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
        }

        public override async Task<MediatorCommandResult<GiftCertificate>> ExecuteAsync()
        {
            var dateStart = DateTime.UtcNow;
            var dateEnd = dateStart.AddMonths(4);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, 1).AddDays(-1);

            int.TryParse(this.Id, out int parsedCertificateId);

            var certificateId = await _giftCertificateRepository.SaveOrUpdateAsync(new GiftCertificate
            {
                Id = parsedCertificateId,
                DateStart = dateStart.Date,
                DateEnd = dateEnd.Date,
                AmountVariantId = AmountVariantId,
                Comment = Comment,
                CertificateSurprises = HasSurprises 
                ? CertificateSurprises.Where(x => x.Quantity > 0).ToList()
                : new List<GiftCertificateSurprise>(),
                Client = new Client
                {
                    Email = Email
                }
            });

            var certificate = await _giftCertificateRepository.GetAsync(certificateId);

            return Success(certificate);
        }
    }
}