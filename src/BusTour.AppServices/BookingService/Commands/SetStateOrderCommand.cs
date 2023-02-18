using BusTour.AppServices.TourOrderProcess;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.BookingService.Commands
{
    public sealed class SetStateOrderCommand : HighLevelMediatorCommand<BaseResponse>
    {
        private readonly string _command;

        private readonly List<int> _ids;

        private readonly ITourOrderProcess _process;

        public SetStateOrderCommand(CommandParameters command)
        {
            _command = command.Command;
            _ids = command.Ids;
            _process = IoC.GetRequiredService<ITourOrderProcess>();
        }

        public async override Task<MediatorCommandResult<BaseResponse>> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_command) || !_ids.Any())
            {
                return Fail("Comand or ids is empty.");
            }

            foreach (var id in _ids)
            {
                _process.Reset();

                await _process.SetContextAsync(id)
;
                await _process.SendCommandAsync(_command);
            }

            return Success(new BaseResponse() { IsSuccess = true });
        }
    }
}
