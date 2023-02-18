using BusTour.AppServices.OrderService.Queries;
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

namespace BusTour.AppServices.OrderService
{

    [InjectAsScoped]
    public class UpgradeOrderCommand : HighLevelMediatorCommand<bool>
    {
        private readonly UpgradeOrderRequest _request;

        private readonly IOrderRepository _orderRepository;
        private readonly IBusRepository _busRepository;

        private Order _order;
        private Bus _bus;

        private int _clickedId => _request.ClickedObject.Id;

        public UpgradeOrderCommand(UpgradeOrderRequest request)
        {
            _request = request;

            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _busRepository   = IoC.GetRequiredService<IBusRepository>();
        }
        
        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            _order = await _orderRepository.GetAsync(_request.OrderId);
            _bus = await _busRepository.GetAsync(_order.Tour.BusId);

            var busModel = (await Mediator.RunQueryAsync(new GetOrderBusModelQuery(_request.OrderId))).Result;
            
            if (_order.TableType == SelectionVariant.SharedTable && _request.ClickedObject.Type == BusObjectTypes.Table)
            {
                _order.TableType = SelectionVariant.IndividualTable;

                _ = _orderRepository.UpdateAsync(_order);

                foreach (var seat in busModel.Tables.First(x => x.Id == _clickedId).Seats)
                {
                    if (!seat.IsSelected)
                    {
                        await _orderRepository.AddOrderSeatAsync(new OrderSeat
                        {
                            OrderId = _request.OrderId,
                            SeatId = seat.Id,
                            Price = seat.Price,
                            IsEmpty = true
                        });
                    }
                }
            }

            else if (IsClickObjectAvailable(busModel) && !IsClickObjectContextChanged(busModel))
            {
                
                await UpdateOrderWithClickObject(busModel);
            }

            return Success(true);
        }

        private bool IsClickObjectAvailable(OrderBusModel busModel)
        {
            if (_request.ClickedObject.Type == BusObjectTypes.Table)
            {
                var table = busModel.Tables.FirstOrDefault(x => x.Id == _clickedId);
                return table.IsAvailable;
            }
            else if (_request.ClickedObject.Type == BusObjectTypes.Seat)
            {
                var seat = busModel.Tables.SelectMany(x => x.Seats).FirstOrDefault(x => x.Id == _clickedId);
                return seat.IsAvailable;
            }
            else
            {
                return true;
            }
        }

        private bool IsClickObjectContextChanged(OrderBusModel busModel)
        {
            //todo: Сделать проверку на то, что контекст принятия решения поменялся
            return false;
        }

        private async Task UpdateOrderWithClickObject(OrderBusModel busModel)
        {
            var changedSeats = new Dictionary<int, decimal>();

            if (_request.ClickedObject.Type == BusObjectTypes.Table)
            {
                var tableModel = busModel.Tables.First(x => x.Id == _clickedId);
                var freeSeats = tableModel.Seats.Where(x => x.IsFree);
                if (freeSeats.Any())
                {
                    var tableSum = tableModel.Price;
                    foreach (var freeSeat in freeSeats)
                    {
                        var seatSum = Math.Min(freeSeat.Price, tableSum);
                        changedSeats.Add(freeSeat.Id, seatSum);
                        tableSum -= seatSum;
                    }
                }
                else
                {
                    changedSeats = tableModel.Seats.ToDictionary(x => x.Id, x => x.Price);
                }
            }
            else if (_request.ClickedObject.Type == BusObjectTypes.Seat)
            {
                var seatModel = busModel.Seats.First(x => x.Id == _clickedId);
                changedSeats.Add(_clickedId, seatModel.Price);
            }

            foreach (var (seatId, price) in changedSeats)
            {
                var orderSeat = _order.Seats.FirstOrDefault(x => x.SeatId == seatId);
                if (orderSeat is null)
                {
                    var seat = busModel.Seats.FirstOrDefault(x => x.Id == seatId);
                    await _orderRepository.AddOrderSeatAsync(new OrderSeat
                    {
                        OrderId = _request.OrderId,
                        SeatId = seatId,
                        Price = price
                    });
                }
                else
                {
                    await _orderRepository.DeleteOrderSeatAsync(orderSeat.Id);
                }
            }
        }
    }
}
