using BusTour.AppServices.TourService;
using BusTour.Common;
using BusTour.Common.Config;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.PromoCodes;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BusTour.AppServices.BookingService.Queries
{
    public sealed class GetCalculationCostTourQuery : MediatorQuery<CalculationCostTourResponse>
    {
        private readonly Order _order;

        private readonly ITourRepository _tourRepository;
        private readonly IBusRepository _busRepository;
        private readonly ITourService _tourService;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IGiftCertificateRepository _giftCertificateRepository;
        private readonly IBookingService _bookingService;
        private readonly int _percentVAT;

        public GetCalculationCostTourQuery()
        {
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _tourService = IoC.GetRequiredService<ITourService>();
            _busRepository = IoC.GetRequiredService<IBusRepository>();
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
            _giftCertificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
            _bookingService = IoC.GetRequiredService<IBookingService>();
            _percentVAT = Config.Get<PercentVATConfig>().Value;
        }

        public GetCalculationCostTourQuery(OrderModel orderModel) : this()
        {
            _order = orderModel != null ? _bookingService.ConvertToEntity(orderModel) : null;
        }

        public GetCalculationCostTourQuery(Order order) : this()
        {
            _order = order;
        }

        private decimal CalculateDiscount(bool isPercent, decimal amount, decimal value)
        {
            return Math.Max(isPercent
                ? value / 100 * (100 - amount)
                : value - amount, 0);
        }

        public override async Task<MediatorCommandResult<CalculationCostTourResponse>> ExecuteAsync()
        {
            if (_order == null)
            {
                return Fail("Order model is empty.");
            }

            var tour = await _tourRepository.GetAsync(_order.TourId);

            if (tour == null)
            {
                return Fail("Tour not found.");
            }

            var result = new CalculationCostTourResponse();

            var menuInfo = await _tourService.GetMenuInfoAsync();

            var seats = await _busRepository.SelectSeatAsync(new SeatFilter { BusIds = new List<int> { tour.BusId } });

            PromoCode promocode = _order.PromoCodeId != null
                ? await _promoCodeRepository.GetAsync((int)_order.PromoCodeId)
                : null;

            GiftCertificate certificate = _order.CertificateId != null
                ? await _giftCertificateRepository.GetAsync((int)_order.CertificateId)
                : null;

            if(_order.TableType == Domain.Enums.SelectionVariant.SharedTable)
            {
                // Сумма стоимости всех сидений без учета промокода
                var defaultSeatPrice = seats.FirstOrDefault().Price;
                var seatPrice = tour.SeatPrice ?? defaultSeatPrice;
                var vipPrice = tour.VipPrice ?? defaultSeatPrice;

                var noVipSeatCount = _order.Seats
                        .Select(x => seats.FirstOrDefault(m => m.Id == x.SeatId))
                        .Where(x => x != null)
                        .DistinctBy(x => x.Id)
                        .Count(x => tour.Tables.Any(t => !t.IsVip && t.Seats.Any(ts => ts.Id == x.Id)));

                var vipSeatCount = _order.Seats
                       .Select(x => seats.FirstOrDefault(m => m.Id == x.SeatId))
                       .Where(x => x != null)
                       .DistinctBy(x => x.Id)
                       .Count(x => tour.Tables.Any(t => t.IsVip && t.Seats.Any(ts => ts.Id == x.Id)));

                result.TourPrice = noVipSeatCount * seatPrice + vipSeatCount * vipPrice;

                //result.TourPrice = _orderModel.Seats.Select(x => seats.FirstOrDefault(m => m.Id == x.SeatId))
                //     .Where(x => x != null)
                //     .DistinctBy(x => x.Id)
                //     .Sum(x => x.Price);
            }
            else if(_order.TableType == Domain.Enums.SelectionVariant.IndividualTable)
            {
                // Сумма стоимости всех столов без учета промокода
                result.TourPrice = _order.Seats.Select(x => tour.Tables.FirstOrDefault(t => t.Seats.Any(s => s.Id == x.SeatId)).Price).Sum();
            }

            // Сумма стоимости всех мест с промокодом
            var tourPrice = promocode == null 
                ? result.TourPrice
                : CalculateDiscount(promocode.TypeOfDiscount == Domain.Enums.TypeOfDiscount.ByPercent, promocode.AmountOfDiscount, result.TourPrice); ;

            var sumMenus = _order.Menus
                .Where(x => x.Amount > 0)
                .Select(x => menuInfo.Menus.FirstOrDefault(m => m.Id == x.MenuId).Price * x.Amount)
                .Sum(x => x);

            var sumBeverages = _order.Beverages
                .Where(x => x.Amount > 0)
                .Select(x => menuInfo.Beverages.FirstOrDefault(b => b.Id == x.BeverageId).Price * x.Amount)
                .Sum(x => x);

            var extrasSummWithVAT = (sumMenus + sumBeverages) / 100 * (100 + _percentVAT);
           

            var totalPriceWithVAT = tourPrice + sumMenus + sumBeverages;

            if (totalPriceWithVAT > 0)
            {
                if (certificate != null)
                {
                    totalPriceWithVAT = CalculateDiscount(false, (decimal)certificate.Amount, totalPriceWithVAT);
                }

                result.TotalPrice = totalPriceWithVAT;
                result.VAT = extrasSummWithVAT / (100 + _percentVAT) * _percentVAT;
            }
            else
            {
                result.TotalPrice = 0;
                result.VAT = 0;
            }

            return Success(result);
        }
    }
}
