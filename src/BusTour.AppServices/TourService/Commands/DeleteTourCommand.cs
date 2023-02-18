using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourProcess;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Commands
{
    public class DeleteTourCommand : HighLevelMediatorCommand<BaseResponse>
    {
        private readonly ITourProcess _tourProcessRepository;
        private readonly ITourOrderProcess _tourOrderProcess;
        private readonly IOrderRepository _orderRepository;

        public DeleteTourCommand()
        {
            _tourProcessRepository = IoC.GetRequiredService<ITourProcess>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourOrderProcess = IoC.GetRequiredService<ITourOrderProcess>();
        }

        public override async Task<MediatorCommandResult<BaseResponse>> ExecuteAsync()
        {
            try
            {
                if (int.TryParse(Id, out var tourId)) {
                    await _tourProcessRepository.SendCommandAsync(tourId, TourStepCommand.Delete);

                    var tourOrders = await _orderRepository.SelectAsync(new OrderFilter
                    {
                        TourIds = new[] { tourId }
                    });

                    foreach (var order in tourOrders)
                    {
                        await _tourOrderProcess.SendCommandAsync(order.Id, TourOrderStepCommand.Cancel);
                    }

                    return Success(new BaseResponse { IsSuccess = true });
                }
                else
                {
                    throw new Exception("Something went wrong...");
                }
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }
    }
}
