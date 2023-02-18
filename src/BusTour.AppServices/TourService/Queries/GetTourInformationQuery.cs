using BusTour.AppServices.BookingService.Queries;
using BusTour.Common.Config;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Tour;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class GetTourInformationQuery : MediatorQuery<TourInformationModel>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;

        private int tourId;

        public GetTourInformationQuery()
        {
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
        }

        public override async Task<MediatorCommandResult<TourInformationModel>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out tourId))
            {
                return Fail("Incorrect tour identifier");
            }

            var model = new TourInformationModel();
            var tourOrders = new List<Order>();

            try
            {   
                tourOrders = await _orderRepository.SelectAsync(new OrderFilter { TourIds = new[] { tourId } });

                var beverages = await _tourRepository.GetBeveragesAsync();
                var menus = await _tourRepository.GetMenusAsync();

                var beveragesSum = tourOrders
                    .SelectMany(x => x.Beverages)
                    .GroupBy(x => x.BeverageId)
                    .Sum(g =>
                    {
                        var beverage = beverages.FirstOrDefault(z => z.Id == g.Key);
                        return beverage != null ? beverage.Price * g.Sum(y => y.Amount) : 0;
                    });

                var menusSum = tourOrders
                    .SelectMany(x => x.Menus)
                    .Where(x => x.MenuId.HasValue)
                    .GroupBy(x => x.MenuId.Value)
                    .Sum(g =>
                    {
                        var menu = menus.FirstOrDefault(z => z.Id == g.Key);
                        return menu != null ? menu.Price * g.Sum(y => y.Amount) : 0;
                    });

                model.TourConflicts = await GetTourOrdersConflicts();
                model.TourOrders = tourOrders.Select(order => new TourOrderGridModel
                {
                    Seats = order.Seats,
                    Client = order.Client,
                    OrderId = order.Id,
                    OrderState = order.OrderState,
                    Conflict = model.TourConflicts.Conflicts.Any(c => c.OrderId == order.Id),
                    Comment = order.Comment,
                    Hash = order.Hash,
                }).ToArray();

                await FillTourSummary(model, tourOrders);

                model.TourSummary.VatPrice = (beveragesSum + menusSum) * (decimal)Config.Get<PercentVATConfig>().Value/100;
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }

            return Success(model);
        }

        private async Task FillTourSummary(TourInformationModel model, List<Order> tourOrders)
        {
            var tour = await _tourRepository.GetAsync(tourId, fillNested: true);

            if (tour == null)
            {
                throw new ArgumentException($"The tour with id = {tourId} does not exists");
            }

            var occupaidSeatsOrderStates = new List<OrderState>() { OrderState.Paid, OrderState.WaitingForPayment };

            var tourSummary = new TourSummaryModel()
            {
                TourId = tourId,
                CityId = tour.Route?.CityId,
                TourType = tour.Type.Value,
                Itinerary = tour.Route?.Name,
                TourState = tour.TourState,
                TourNumber = tour.Number,
                DepartureDateTime = tour.Departure,
                ArrivalDateTime = tour.Departure + (tour?.Duration ?? new TimeSpan(0, 0, 0)),
                Duration = tour.Duration ?? new TimeSpan(0, 0, 0),
                Conflict = model.TourConflicts.Conflicts.Any(),
                Occupaid = tour.PrivateHire?.GuestCount ?? tourOrders
                    .Where(order => order.OrderState.HasValue && occupaidSeatsOrderStates.Contains(order.OrderState.Value))
                    .Sum(order => order.Seats.Count),
                SeatsCount = tour.Tables.Sum(x => x.Seats.Count()),
                UsedGiftsCount = tourOrders.Where(o => o.CertificateId.HasValue).Sum(o => o.Id),
                UserPromoCodesCount = tourOrders.Where(o => o.PromoCodeId.HasValue).Sum(o => o.Id),
                Extras = tourOrders.Any(o => o.Beverages.Any() || o.Menus.Any()),
                TourPaymentInformation = tourOrders.Where(o => o.OrderState != null && o.OrderState == OrderState.Paid).Sum(o => o.Id),
                TotalPrice = tourOrders.Sum(o => o.TotalPrice),
            };

            model.TourSummary = tourSummary;
        }

        private async Task<OrdersConflictsResponse> GetTourOrdersConflicts()
        {
            return new OrdersConflictsResponse((await Mediator.RunCommandAsync(
                new CheckOrdersConflictsQuery(
                    new OrderFilter { 
                        TourIds = new[] { tourId },
                        States = new[] { OrderState.WaitingForPayment, OrderState.Paid, OrderState.Draft }
                    }
                )
            )).Result);
        }
    }
}
