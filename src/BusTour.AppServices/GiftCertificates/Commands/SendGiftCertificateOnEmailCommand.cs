using BusTour.AppServices.Notifications;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.NotificationEvents;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.AppServices.GiftCertificates.Commands
{
    [InjectAsScoped]
    public class SendGiftCertificateOnEmailCommand : HighLevelMediatorCommand<GiftCertificate>
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


        public SendGiftCertificateOnEmailCommand()
        {
            _giftCertificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
        }

        public override async Task<MediatorCommandResult<GiftCertificate>> ExecuteAsync()
        {
            int.TryParse(this.Id, out int parsedCertificateId);

            var certificate = await _giftCertificateRepository.GetAsync(parsedCertificateId);

            certificate.Client.Email = Email;

            await _giftCertificateRepository.SaveOrUpdateAsync(certificate);

            await _notificationServiсe.AddNotificationAsync(new GiftCertificateAddedNotificationEvent(certificate));

            return Success(certificate);
        }
    }
}