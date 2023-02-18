using BusTour.AppServices.TourProcess;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Common.Context;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BusTour.Domain.Models.Filters;
using BusTour.Common;
using BusTour.Domain.Models.Responses;
using BusTour.Data.Repositories.Orders;
using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.AppServices.TourService.Queries;

namespace BusTour.AppServices.TourService.Commands
{
    [InjectAsScoped]
    public class CreateToursCommand : HighLevelMediatorCommand<List<Tour>, TourCreationFailResponse>
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool ChooseWeekdays { get; set; }
        public TourType Type { get; set; }
        public int? RouteId { get; set; }
        public List<CreateToursTour> Tours { get; set; }

        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITourOrderProcess _orderProcess;
        private readonly ITourProcess _tourProcess;

        private int _busId;

        public CreateToursCommand()
        {
            _tourRepository  = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderProcess    = IoC.GetRequiredService<ITourOrderProcess>();
            _tourProcess     = IoC.GetRequiredService<ITourProcess>();
        }

        public override async Task<MediatorCommandResult<List<Tour>>> ExecuteAsync()
        {
            _busId = (await _tourRepository.GetBusesAsync()).First().Id;
            RouteId = RouteId ?? (await _tourRepository.GetRoutesAsync()).First().Id;

            if (Type == TourType.Regular)
            {
                return await CreateRegularTours();
            }
            else if (Type == TourType.Service)
            {
                return await CreateServiceTour();
            }
            else
            {
                return Fail("Wrong tour type");
            }

        }

        private async Task<MediatorCommandResult<List<Tour>>> CreateRegularTours()
        {
            List<Tour> tours = new List<Tour>();

            var iterateDate = this.DateStart.Date;

            while (iterateDate <= DateEnd.Date)
            {
                foreach(var createTour in Tours.Where(x => !ChooseWeekdays || (x.WeekdayStart <= iterateDate.DayOfWeek && x.WeekdayEnd >= iterateDate.DayOfWeek)))
                {
                    foreach(var time in createTour.Times.Where(x => x.HasValue).Select(x => x.Value))
                    {
                        var tour = new Tour
                        {
                            Departure = iterateDate.AddHours(time.Hours).AddMinutes(time.Minutes).AddSeconds(time.Seconds),
                            SeatPrice = createTour.SeatPrice,
                            VipPrice = createTour.VipPrice,
                            Discount = createTour.Discount,
                            Type = Type,
                            RouteId = RouteId,
                            BusId = _busId
                        };
                        if (createTour.HasMenu)
                        {
                            tour.TourMenus = createTour.Menus.Concat(createTour.MenusExtra).Distinct().Select(x => new TourMenu 
                            { 
                                MenuId = x,
                                IsTicket = createTour.Menus.Contains(x),
                                IsExtra = createTour.MenusExtra.Contains(x)
                            }).ToList();
                        }
                        if (createTour.HasBeverages)
                        {
                            tour.TourBeverages = createTour.Beverages.Concat(createTour.BeveragesExtra).Distinct().Select(x => new TourBeverage 
                            { 
                                BeverageId = x,
                                IsTicket = createTour.Beverages.Contains(x),
                                IsExtra = createTour.BeveragesExtra.Contains(x)
                            }).ToList();
                        }
                        tours.Add(tour);
                    }
                }
                iterateDate = iterateDate.AddDays(1);
            }

            foreach (var tour in tours)
            {
                await _tourRepository.SaveOrUpdateAsync(tour);
            }

            tours = await _tourRepository.SelectAsync(new TourFilter { Ids = tours.Select(x => x.Id) });

            //Валидируем
            var blockingTours = new List<Tour>();
            var duplicateTours = new List<Tour>();

            foreach (var tour in tours)
            {
                var crossingTours = (await Mediator.RunCommandAsync(new GetCrossingToursQuery(tour))).Result;

                blockingTours.AddRange(crossingTours.Where(x => x.Type > tour.Type));
                duplicateTours.AddRange(crossingTours.Where(x => x.TourState == TourState.Active && x.Type <= tour.Type));
            }

            if (duplicateTours.Any() || blockingTours.Any())
            {
                return Fail(new TourCreationFailResponse
                {
                    DuplicateTours = duplicateTours.DistinctBy(x => x.Id),
                    BlockingTours  = blockingTours.DistinctBy(x => x.Id)
                });
            }

            foreach (var tour in tours)
            {
                _tourProcess.Reset();
                await _tourProcess.SetContextAsync(tour.Id);
                await _tourProcess.InitStateAsync();
                await _tourProcess.SendCommandAsync(TourStepCommand.Active);
            }

            tours = await _tourRepository.SelectAsync(new TourFilter { Ids = tours.Select(x => x.Id) });

            return Success(tours);
        }

        private async Task<MediatorCommandResult<List<Tour>>> CreateServiceTour()
        {
            //Собираем все даты
            var tourDates = new List<(DateTime serviceStart, DateTime serviceEnd)> ();

            var iterateDate = DateStart.Date;

            while (iterateDate <= DateEnd.Date)
            {
                foreach (var createTour in Tours
                    .Where(x => x.ServiceStart.HasValue && x.ServiceEnd.HasValue)
                    .Where(x => !ChooseWeekdays || (x.WeekdayStart <= iterateDate.DayOfWeek && x.WeekdayEnd >= iterateDate.DayOfWeek))
                    )
                {
                    tourDates.Add(( createTour.ServiceStart.Value.AddToDate(iterateDate), createTour.ServiceEnd.Value.AddToDate(iterateDate)));
                }
                iterateDate = iterateDate.AddDays(1);
            }

            tourDates = tourDates.DistinctBy(x => x.serviceStart).ToList();

            //Создаем туры
            var tourIds = new List<int>();

            foreach (var tourDate in tourDates)
            {
                var tour = new Tour
                {
                    BusId = _busId,
                    Type = TourType.Service,
                    RouteId = RouteId,
                    Departure = tourDate.serviceStart,
                    ServiceMaintenance = new TourServiceMaintenance
                    {
                        Duration = tourDate.serviceEnd - tourDate.serviceStart
                    }
                };

                tourIds.Add(await _tourRepository.SaveOrUpdateAsync(tour));
            }

            var tours = await _tourRepository.SelectAsync(new TourFilter { Ids = tourIds });

            //Валидируем
            var blockingTours = new List<Tour>();

            foreach (var tour in tours)
            {
                var crossingTours = (await Mediator.RunCommandAsync(new GetCrossingToursQuery(tour))).Result;
                blockingTours.AddRange(crossingTours.Where(x => x.TourState == TourState.Active && x.Type >= tour.Type));
            }

            if (blockingTours.Any())
            {
                return Fail(new TourCreationFailResponse { BlockingTours = blockingTours.DistinctBy(x => x.Id) });
            }

            //Создаем заказы и двигаем по статусам
            foreach (var tour in tours)
            {
                _tourProcess.Reset();
                await _tourProcess.SetContextAsync(tour.Id);
                await _tourProcess.InitStateAsync();
                await _tourProcess.SendCommandAsync(TourStepCommand.Active);

                var order = new Order
                {
                    TourId = tour.Id,
                    OrderDate = DateTime.UtcNow
                };
                await _orderRepository.SaveOrUpdateAsync(order);

                _orderProcess.Reset();
                await _orderProcess.SetContextAsync(order.Id);
                await _orderProcess.SendCommandAsync(TourOrderStepCommand.WaitingForPaiment);
                await _orderProcess.SendCommandAsync(new PayStepCommandArgs(TourOrderStepCommand.Payment) { IsPaid = true });

                await new TourCommandsHelpers().TryCancelCrossedTours(tour);
            }

            tours = await _tourRepository.SelectAsync(new TourFilter { Ids = tourIds });

            return Success(tours);
        }

        public class CreateToursTour
        {
            public decimal? SeatPrice { get; set; }
            public decimal? VipPrice { get; set; }
            public decimal? Discount { get; set; }
            public DayOfWeek WeekdayStart { get; set; }
            public DayOfWeek WeekdayEnd { get; set; }
            public bool HasMenu { get; set; }
            public bool HasBeverages { get; set; }
            public List<int> Menus { get; set; }
            public List<int> Beverages { get; set; }
            public List<int> MenusExtra { get; set; }
            public List<int> BeveragesExtra { get; set; }
            public List<Time?> Times { get; set; }
            public Time? ServiceStart { get; set; }
            public Time? ServiceEnd { get; set; }

            public CreateToursTour()
            {
                Menus = new List<int>();
                Beverages = new List<int>();
                Times = new List<Time?>();
                MenusExtra = new List<int>();
                BeveragesExtra = new List<int>();
            }
        }
    }
}