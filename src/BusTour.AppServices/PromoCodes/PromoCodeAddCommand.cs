using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.PromoCodes
{
    [InjectAsScoped]
    public class PromoCodeAddCommand : HighLevelMediatorCommand<PromoCode>
    {
        /// <summary>
        /// Вид Промокода
        /// </summary>
        public PromoCodeType PromoCodeType { get; set; }

        /// <summary>
        /// Серия номер промокода
        /// </summary>
        public string SeriesNumber { get; set; }

        /// <summary>
        /// Дата начала действия промокода
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// Дата окончания действия промокода
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Кол-во промокодов
        /// </summary>
        public int? NumberOfPromocodes { get; set; }

        /// <summary>
        /// Размер скидки
        /// </summary>
        public decimal AmountOfDiscount { get; set; }

        /// <summary>
        /// Тип скидки
        /// </summary>
        public TypeOfDiscount TypeOfDiscount { get; set; }

        /// <summary>
        /// Id города
        /// </summary>
        public int CityId { get; set; }

        private readonly IPromoCodeRepository _promoCodeRepository;

        public PromoCodeAddCommand()
        {
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public override async Task<MediatorCommandResult<PromoCode>> ExecuteAsync()
        {
            if (await _promoCodeRepository.GetByFilterAsync(new PromoCodeFilter { SeriesNumber = SeriesNumber }) != null) {
                return Fail($"Promo code with series number \"{SeriesNumber}\" already exists");
            }

            DateTime? dateStart = DateStart.HasValue ?
                DateStart.ToString().ToUtcDateTime() : null;

            DateTime? dateEnd = DateEnd.HasValue ?
                DateEnd.ToString().ToUtcDateTime() : null;

            var promoCodeId = await _promoCodeRepository.SaveOrUpdateAsync(new PromoCode
            {
                AmountOfDiscount = AmountOfDiscount,
                DateStart = dateStart,
                DateEnd = dateEnd,
                NumberOfPromocodes = NumberOfPromocodes,
                PromoCodeType = PromoCodeType,
                TypeOfDiscount = TypeOfDiscount,
                SeriesNumber = SeriesNumber,
                CreateDate = DateTime.UtcNow,
                CityId = CityId,
                IsActive = true,
            });

            var promoCode = await _promoCodeRepository.GetAsync(promoCodeId);

            return Success(promoCode);
        }
    }
}
