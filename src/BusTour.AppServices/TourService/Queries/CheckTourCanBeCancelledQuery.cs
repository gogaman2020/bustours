using BusTour.AppServices.BookingService.Queries;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Tour;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class CheckTourCanBeCancelledQuery : MediatorQuery<bool>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;

        private int _tourId;

        public CheckTourCanBeCancelledQuery(int tourId)
        {
            _tourId = tourId;
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            var paidOrder = await _orderRepository.SelectAsync(new OrderFilter { 
                TourIds = new[] { _tourId },
                States = new List<OrderState> { OrderState.Paid }
            });

            return Success(!paidOrder.Any());
        }
    }
}
