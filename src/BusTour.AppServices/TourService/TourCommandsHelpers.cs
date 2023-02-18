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

namespace BusTour.AppServices.TourService
{
    public class TourCommandsHelpers
    {
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITourOrderProcess _orderProcess;
        private readonly ITourProcess _tourProcess;
        private readonly IMediator _mediator;

        public TourCommandsHelpers()
        {
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderProcess = IoC.GetRequiredService<ITourOrderProcess>();
            _tourProcess = IoC.GetRequiredService<ITourProcess>();
            _mediator = IoC.GetRequiredService<IMediator>();
        }

        /// <summary>
        /// Отмена туров ожидающих отмены
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> TryCancelCancelRequestsTours()
        {
            var tourIds = new List<int>();

            var cancelRequestTours = await _tourRepository.SelectAsync(new TourFilter
            {
                States = new List<TourState> { TourState.CancelRequest }
            });

            foreach (var tour in cancelRequestTours)
            {
                if ((await _mediator.RunQueryAsync(new CheckTourCanBeCancelledQuery(tour.Id))).Result)
                {
                    tourIds.Add(tour.Id);
                    await _tourProcess.SendCommandAsync(tour.Id, TourStepCommand.Cancel);
                }
            }

            return tourIds;
        }

        /// <summary>
        /// Отмена пересекающихся туров
        /// </summary>
        /// <param name="baseTour"></param>
        /// <returns></returns>
        public async Task<List<int>> TryCancelCrossedTours(Tour baseTour)
        {
            var tourIds = new List<int>();

            var crossingTours = (await _mediator.RunQueryAsync(new GetCrossingToursQuery(baseTour))).Result;

            foreach (var crossingTour in crossingTours.Where(x => x.Type < baseTour.Type))
            {
                if (crossingTour.TourState == TourState.Active)
                {
                    if ((await _mediator.RunQueryAsync(new CheckTourCanBeCancelledQuery(crossingTour.Id))).Result)
                    {
                        tourIds.Add(crossingTour.Id);
                        await _tourProcess.SendCommandAsync(crossingTour.Id, TourStepCommand.Cancel);

                        var tourOrders = await _orderRepository.SelectAsync(new OrderFilter
                        {
                            TourIds = new List<int> { crossingTour.Id }
                        });

                        foreach (var tourOrder in tourOrders.Where(x => x.OrderState <= OrderState.WaitingForPayment))
                        {
                            await _orderProcess.SendCommandAsync(tourOrder.Id, TourOrderStepCommand.Cancel);
                        }
                    }
                    else
                    {
                        tourIds.Add(crossingTour.Id);
                        await _tourProcess.SendCommandAsync(crossingTour.Id, TourStepCommand.CancelRequest);
                    }
                }
                else if (crossingTour.TourState == TourState.Draft)
                {
                    tourIds.Add(crossingTour.Id);
                    await _tourProcess.SendCommandAsync(crossingTour.Id, TourStepCommand.Delete);
                }
            }

            return tourIds;
        }
    }
}
