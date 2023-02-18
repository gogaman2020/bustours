using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.PromoCodes
{
    public class ValidatePromoCodeCommand : HighLevelMediatorCommand<PromoCode>
    {
        public string SeriesNumber { get; set; }
        public int CityId { get; set; }

        private readonly IPromoCodeRepository _promoCodeRepository;

        public ValidatePromoCodeCommand()
        {
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public async override Task<MediatorCommandResult<PromoCode>> ExecuteAsync()
        {
            var promo = await _promoCodeRepository.GetByFilterAsync(new PromoCodeFilter { SeriesNumber = SeriesNumber });

            if (promo == null)
            {
                return Fail($"Promo code with series number {SeriesNumber} not found");
            }

            var conflictsList = new List<string>();

            // валидация на количество промокодов
            if (!promo.IsActive)
            {
                conflictsList.Add($"Promo code with series number {SeriesNumber} inactive");
            }


            // валидация на количество промокодов
            if (promo.NumberOfPromocodes.HasValue && promo.NumberOfUses >= promo.NumberOfPromocodes)
            {
                conflictsList.Add($"Promo code with series number {SeriesNumber} is not valid");
            }

            var dateNow = DateTime.UtcNow;

            // валидация на дату старта действия промокода
            if (promo.DateStart > dateNow)
            {
                conflictsList.Add($"Promo code with series number {SeriesNumber} has not entered into force");
            }

            // валидация на дату окончания действия промокода
            if (promo.DateEnd < dateNow)
            {
                conflictsList.Add($"The validity period of the promo code with series number {SeriesNumber} has ended");
            }

            // валидация на город в котором действует промокод
            if (promo.CityId != CityId)
            {
                conflictsList.Add($"The city is not valid");
            }

            if (conflictsList.Any())
            {
                return Fail(string.Join(". ", conflictsList));
            }

            return Success(promo);
        }
    }
}
