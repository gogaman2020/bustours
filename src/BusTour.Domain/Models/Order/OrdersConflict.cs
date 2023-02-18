using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.Order
{
    public class OrdersConflictWithoutBase
    {
        /// <summary>
        /// Конфликтующие заказы
        /// </summary>
        public DomainOrder ConflictOrder { get; set; }

        /// <summary>
        /// Конфликтные места
        /// </summary>
        public List<int> ConflictSeatIds { get; set; }

        /// <summary>
        /// Тип конфликта
        /// </summary>
        public OrdersConflictType Type => ConflictSeatIds.Any() ? OrdersConflictType.Seat : OrdersConflictType.Bus;

        public OrdersConflictWithoutBase(DomainOrder conflictOrder, IEnumerable<int> seatIds)
        {
            ConflictOrder = conflictOrder;
            ConflictSeatIds = seatIds.ToList();
        }
    }

    public class OrdersConflict : OrdersConflictWithoutBase
    {
        /// <summary>
        /// Блокирующий конфликт
        /// </summary>
        public bool IsBlocking => ConflictOrder.Type >= _baseOrder.Type && ConflictOrder.OrderState == OrderState.Paid;

        /// <summary>
        /// Необходимо одобрение
        /// </summary>
        public bool NeedsApprovement => CanBeCancelled && ConflictOrder.OrderState == OrderState.Paid;

        /// <summary>
        /// Можно отменить
        /// </summary>
        public bool CanBeCancelled => ConflictOrder.Type < _baseOrder.Type;

        private DomainOrder _baseOrder;

        public OrdersConflict(DomainOrder baseOrder, DomainOrder conflictOrder, IEnumerable<int> seatIds) 
            : base(conflictOrder, seatIds)
        {
            _baseOrder = baseOrder;
        }
    }
}
