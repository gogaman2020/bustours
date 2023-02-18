using BusTour.AppServices.Notifications;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.NotificationEvents;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Common.Configs;
using BusTour.Common.Config;
using BusTour.AppServices.BookingService.Queries;
using BusTour.AppServices.BookingService;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class AddNotificationCommand : HighLevelMediatorCommand<bool>
    {
        private readonly IOrderRepository _tourOrderRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBusRepository _busRepository;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly Urls _urls;
        private readonly int _orderId;
        private readonly string _email;
        private readonly string _language;

        public AddNotificationCommand(int orderId, string email, string language)
        {
            _tourOrderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _busRepository = IoC.GetRequiredService<IBusRepository>();
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
            _urls = Config.Get<Urls>();
            _orderId = orderId;
            _email = email;
            _language = language;
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            var notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
            var order = await _orderRepository.GetAsync(_orderId);
            if (order == null)
            {
                return Fail($"Order with id: {_orderId} not found");
            }
            var seats = await _busRepository.SelectSeatAsync(new SeatFilter { BusIds = new List<int> { order.Tour.BusId }});
            var menus = await _tourRepository.GetMenusAsync();
            var beverages = await _tourRepository.GetBeveragesAsync();
            var route = _tourRepository.GetRouteAsync((short)order.Tour.RouteId);
            PromoCode promocode = order.PromoCodeId != null
                ? await _promoCodeRepository.GetAsync((int)order.PromoCodeId)
                : null;
            //order.Tour = await _tourRepository.GetAsync(order.TourId, fillNested: true);

            var cost = (await Mediator.RunQueryAsync(new GetCalculationCostTourQuery(order))).Result;

            order.TotalPrice = cost.TotalPrice;

            await notificationServiсe.AddNotificationAsync(new BookingSummaryNotificationEvent(
                order, 
                seats, 
                menus, 
                beverages, 
                route, 
                promocode,  
                _email, 
                _language, 
                _urls.Site
            ));

            return Success(true);
        }
    }
}
