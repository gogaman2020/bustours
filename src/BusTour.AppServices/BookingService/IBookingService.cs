using BusTour.Domain.Entities;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using System.Threading.Tasks;

namespace BusTour.AppServices.BookingService
{
    public interface IBookingService
    {
        Order ConvertToEntity(OrderModel model);
    }
}
