using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Commands
{
    public class UpdateServiceTourCommand : HighLevelMediatorCommand<BaseResponse>
    {
        private ITourRepository _tourRepository;

        public UpdateServiceTourCommand(Tour tour)
        {
            Tour = tour;
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
        }

        private readonly Tour Tour;

        public override async Task<MediatorCommandResult<BaseResponse>> ExecuteAsync()
        {
            try
            {
                await _tourRepository.UpdateServiceTourAsync(Tour);
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }

            return Success(new BaseResponse { IsSuccess = true });
        }
    }
}
