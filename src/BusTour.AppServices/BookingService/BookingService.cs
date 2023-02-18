using BusTour.Common.Config;
using BusTour.Data.Repositories;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.BookingService
{
    [InjectAsSingleton]
    public class BookingService : IBookingService
    {
        private readonly ApiConfig _apiConfig;

        private readonly ILogger _logger;

        private readonly IOrderRepository _orderRepository;

        public BookingService(IOrderRepository orderRepository)
        {
            _apiConfig = Config.Get<ApiConfig>();
            _logger = LogManager.GetCurrentClassLogger();
            _orderRepository = orderRepository;
        }

        public Order ConvertToEntity(OrderModel model)
        {
            var result = new Order 
                {
                    Id            = model.Id,
                    TourId        = model.TourId,
                    OrderDate     = DateTime.UtcNow,
                    PaymentDate   = null,
                    PromoCodeId   = model.PromoCodeId,
                    CertificateId = model.CertificateId,
                    Discount      = null, // ToDo
                    TotalPrice    = model.TotalPrice,
                    Client        = model.Client!=null ? ConvertToEntity(model.Client) : null,
                    Seats         = model.Seats.Select(ConvertToEntity).ToList(),
                    Menus         = model.Menus.Select(ConvertToEntity).ToList(),
                    Beverages     = model.Beverages.Select(ConvertToEntity).ToList(),
                    Surprises     = model.Surpises.Select(ConvertToEntity).ToList(),
                    Comment         = model.Comment,
                    SpecialRequests = model.SpecialRequests,
                    IsGroup         = model.IsGroup,
                    GuestCount      = model.GuestCount,
                    DisabledGuestCount = model.DisabledGuestCount,
                    TableType       = model.SeatingType
                };

            //if (model.Tables?.Any() == true)
            //{
            //    var tableIds = model.Tables.Select(p => p.TableId).ToList();

            //    var seatIds = await _orderRepository.GetSeatsByTablesAsync(tableIds);

            //    seatIds.RemoveAll(seatId => result.Seats.Any(p => p.SeatId == seatId));

            //    foreach (var seatId in seatIds)
            //    {
            //        var emptySeat = new OrderSeat
            //            {
            //                SeatId  = seatId,
            //                MenuId  = null,
            //                BeverageId = null,
            //                AllergyId = null,
            //                OtherAllergy = null,
            //                IsEmpty = true
            //            };

            //        result.Seats.Add(emptySeat);
            //    }
            //}

            return result;
        }

        private Client ConvertToEntity(OrderClientModel model)
        {
            var result = new Client
                {
                    Id          = model.Id,
                    Email       = model.Email,
                    FullName    = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    IsSigned    = model.IsSigned
                };

            return result;
        }

        private OrderSeat ConvertToEntity(OrderSeatModel model)
        {
            var result = new OrderSeat
                {
                    SeatId  = model.SeatId,
                    MenuId  = model.MenuId,
                    BeverageId = model.BeverageId,
                    AllergyId = model.AllergyId,
                    OtherAllergy = model.OtherAllergy,
                    IsEmpty = null
                };

            return result;
        }

        private OrderMenu ConvertToEntity(OrderMenuModel model)
        {
            var result = new OrderMenu
                {
                    MenuId = model.MenuId,
                    Amount = model.Amount
                };

            return result;
        }

        private OrderBeverage ConvertToEntity(OrderBeverageModel model)
        {
            var result = new OrderBeverage
                {
                    BeverageId = model.BeverageId,
                    Amount     = model.Amount
                };

            return result;
        }

        private OrderSurprise ConvertToEntity(OrderSurpriseModel model)
        {
            var result = new OrderSurprise
                {
                    SurpriseId = model.SurpriseId,
                    Amount     = model.Amount
                };

            return result;
        }
    }
}
