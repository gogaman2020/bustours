using BusTour.Domain.Models.Order;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Ошибка с действием с заказом.
    /// </summary>
    public class OrderCreationFailResponse : BaseResponse
    { 
        /// <summary>
        /// Блокирующие конфликты
        /// </summary>
        public IEnumerable<OrdersConflict> BlockingConflicts { get; set; }
    }
}
