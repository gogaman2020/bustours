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
    public class GetCrossingToursQuery : MediatorQuery<List<Tour>>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;

        private Tour _tour;

        protected GetCrossingToursQuery()
        {
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
        }

        public GetCrossingToursQuery(Tour tour) : this()
        {
            _tour = tour;
        }

        public override async Task<MediatorCommandResult<List<Tour>>> ExecuteAsync()
        {
            (DateTime start, DateTime end) period = (_tour.Departure, _tour.Arrival);
            
            if (_tour.PrivateHire != null)
            {
                period = (_tour.PrivateHire.BlockBookingDateFrom, _tour.PrivateHire.BlockBookingDateTo);
            }

            var result = await _tourRepository.SelectAsync(new TourFilter
            {
                DepartureDateTo = period.end,
                ArrivalDateFrom = period.start,
                BlockBookingDateFromEnd = period.end,
                BlockBookingDateToStart = period.start,
                BusId = _tour.BusId,
                States = new List<TourState> { TourState.Draft, TourState.Active, TourState.CancelRequest }
            });

            return Success(result.Where(x => x.Id != _tour.Id).ToList());
        }
    }
}
