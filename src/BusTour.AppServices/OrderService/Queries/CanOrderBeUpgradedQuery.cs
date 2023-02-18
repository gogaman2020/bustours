using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Selections;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService.Queries
{

    /// <summary>
    /// Можно ли проапдейтить заказ.
    /// </summary>
    [InjectAsScoped]
    public class CanOrderBeUpgradedQuery : MediatorQuery<bool>
    {
        private readonly int _orderId;

        public CanOrderBeUpgradedQuery(int orderId)
        {
            _orderId = orderId;
        }
        
        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            var busModel = (await Mediator.RunQueryAsync(new GetOrderBusModelQuery(_orderId))).Result;

            return Success(busModel.Tables.Any(x => x.IsAvailable && !x.IsSelected) || busModel.Seats.Any(x => x.IsAvailable && !x.IsSelected));
        }
    }
}
