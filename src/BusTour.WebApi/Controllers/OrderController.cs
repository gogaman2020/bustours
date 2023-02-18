using BusTour.AppServices.BookingService;
using BusTour.AppServices.BookingService.Commands;
using BusTour.AppServices.OrderService;
using BusTour.AppServices.BookingService.Queries;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourService.Commands;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Models.Selection;
using BusTour.AppServices.OrderService.Queries;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class OrderController : BusTourControllerBase
    {
        private readonly IBookingService _bookingService;

        public OrderController(IBookingService bookingService) : base()
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Route("create-update-draft")]
        public Task<ActionResult<OrderResponse>> CreateUpdateDraft(OrderModel orderModel)
        {
            return RunCommandAsync(new CreateOrUpdateDraftOrderCommand(orderModel));
        }

        [HttpPost]
        [Route("set-for-payment")]
        public Task<ActionResult<BaseResponse>> SetForPayment(OrderModel orderModel)
        {
            return RunCommandAsync(new SetStateOrderCommand(new CommandParameters 
            { 
                Ids = new List<int> { orderModel.Id },  
                Command = TourOrderStepCommand.WaitingForPaiment
            }));
        }

        [HttpPost]
        [Route("command-post")]
        public Task<ActionResult<BaseResponse>> SetStateOrder([FromBody] CommandParameters command)
        {
            return RunCommandAsync(new SetStateOrderCommand(command));
        }

        [HttpPost]
        [Route("CreatePrivateHireOrder")]
        public Task<ActionResult<Order>> CreatePrivateHireOrder(OrderModel orderModel)
        {
            return RunCommandAsync(new CreateOrUpdatePrivateHireOrderCommand(orderModel));
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ActionResult<Order>> GetOrder(int id)
        {
            return RunCommandAsync(new GetOrderCommand { Id = id.ToString() });
        }

        [HttpGet]
        [Route("ByHash/{hash}")]
        public Task<ActionResult<Order>> GetOrderByHash(string hash)
        {
            return RunCommandAsync(new GetOrderQuery(new OrderFilter { Hashes = new List<string> { hash } }));
        }

        [HttpGet]
        [Route("Extras/{id}")]
        public Task<ActionResult<OrderExtras>> GetExtras(int id)
        {
            return RunCommandAsync(new GetOrderExtrasCommand { Id = id.ToString() });
        }

        [HttpPost]
        [Route("get-conflicts")]
        public async Task<OrdersConflictsResponse> GetConflicts(OrderModel orderModel)
        {
            var queryResult = await RunCommandAsync(new CheckOrderConflictsQuery(orderModel));
            return new OrdersConflictsResponse((queryResult.Result as dynamic).Value);
        }

        [HttpGet("get-conflicts/{orderId:int}")]
        public async Task<OrdersConflictsResponse> GetConflicts(int orderId)
        {
            var queryResult = await RunCommandAsync(new CheckOrderConflictsQuery(orderId));
            return new OrdersConflictsResponse((queryResult.Result as dynamic).Value);
        }

        [HttpPost]
        [Route("CheckOrdersConflictsByFilter")]
        public async Task<OrdersConflictsResponse> CheckOrdersConflictsByFilter(OrderFilter orderFilter)
        {
            var queryResult = await RunCommandAsync(new CheckOrdersConflictsQuery(orderFilter));
            return new OrdersConflictsResponse((queryResult.Result as dynamic).Value);
        }

        [HttpPost]
        [Route("Cancel")]
        public Task<ActionResult<bool>> Cancel(CancelOrderCommand command)
        {
            return RunCommandAsync(command);
        }

        [HttpPut]
        [Route("UpdateOrderSeat")]
        public Task<ActionResult<OrderSeat>> UpdateOrderSeat(OrderSeat orderSeat)
        {
            return RunCommandAsync(new UpdateOrderSeatCommand(orderSeat));
        }

        [HttpPut]
        [Route("UpdateOrderBeverage")]
        public Task<ActionResult<OrderBeverage>> UpdateOrderBeverage(OrderBeverage orderBeverage)
        {
            return RunCommandAsync(new UpdateOrderBeverageCommand(orderBeverage));
        }

        [HttpPut]
        [Route("UpdateOrderMenu")]
        public Task<ActionResult<OrderMenu>> UpdateOrderMenu(OrderMenu orderMenu)
        {
            return RunCommandAsync(new UpdateOrderMenuCommand(orderMenu));
        }

        [HttpPut]
        [Route("AllGuestHasCome/{orderId}")]
        public Task<ActionResult<bool>> AllGuestHasCome(int orderId)
        {
            return RunCommandAsync(new AllGuestHasComeCommand(orderId));
        }

        [HttpGet("can-be-upgraded/{orderId:int}")]
        public Task<ActionResult<bool>> CanBeUpgraded(int orderId)
        {
            return RunCommandAsync(new CanOrderBeUpgradedQuery(orderId));
        }

        [HttpGet("get-bus-model/{orderId:int}")]
        public Task<ActionResult<OrderBusModel>> GetBusModel(int orderId)
        {
            return RunCommandAsync(new GetOrderBusModelQuery(orderId));
        }

        [HttpPost]
        [Route("upgrade")]
        public async Task<ActionResult<OrderBusModel>> UpgradeOrder(UpgradeOrderRequest request)
        {
            await RunCommandAsync(new UpgradeOrderCommand(request));
            return await RunCommandAsync(new GetOrderBusModelQuery(request.OrderId));
        }
        
        [HttpPost]
        [Route("GetCalculationCostTour")]
        public async Task<ActionResult<CalculationCostTourResponse>> GetCalculationCostTour(OrderModel orderModel)
        {
            return await RunCommandAsync(new GetCalculationCostTourQuery(orderModel));
        }

        [HttpPost]
        [Route("AddNotification")]
        public async Task<ActionResult<bool>> AddNotification(SendModel model)
        {
            return await RunCommandAsync(new AddNotificationCommand(model.OrderId, model.Email, model.Lang));
        }
    }
    public class SendModel
    {
        public int OrderId { get; set; }
        public string Email { get; set; }

        public string Lang { get; set; }
    }
}
