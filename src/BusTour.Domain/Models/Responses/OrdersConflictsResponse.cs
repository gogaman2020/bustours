using BusTour.Domain.Entities;
using Infrastructure.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainOrder = BusTour.Domain.Entities.Order;

namespace BusTour.Domain.Models.Order
{
    /// <summary>
    /// Результат поиска конфликтов
    /// </summary>
    public class OrdersConflictsResponse
    {
        public List<DomainOrder> Orders { get; set; }

        public List<OrderConflictsResponse> Conflicts { get; set; }


        public OrdersConflictsResponse()
        {
            Conflicts = new List<OrderConflictsResponse>();
        }

        public OrdersConflictsResponse(List<OrdersConflict> ordersConflicts) : this()
        {
            ordersConflicts = ordersConflicts.Where(x => x.NeedsApprovement || x.IsBlocking).ToList();

            this.Orders = ordersConflicts.Select(x => x.ConflictOrder).DistinctBy(x => x.Id).ToList();

            this.Conflicts = ordersConflicts.Select(x => new OrderConflictsResponse
            {
                OrderId = x.ConflictOrder.Id,
                SeatIds = x.ConflictSeatIds,
                IsBlocking = x.IsBlocking,
                NeedsApprovement = x.NeedsApprovement,
                CanBeCancelled = x.CanBeCancelled
            }).ToList();
        }

        public OrdersConflictsResponse(List<OrdersConflictWithoutBase> ordersConflicts) : this()
        {
            this.Orders = ordersConflicts.Select(x => x.ConflictOrder).DistinctBy(x => x.Id).ToList();

            this.Conflicts = ordersConflicts.Select(x => new OrderConflictsResponse
            {
                OrderId = x.ConflictOrder.Id,
                SeatIds = x.ConflictSeatIds
            }).ToList();
        }

        public class OrderConflictsResponse
        {
            public int OrderId { get; set; }

            public List<int> SeatIds { get; set; }

            public bool? IsBlocking { get; set; }

            public bool? NeedsApprovement { get; set; }

            public bool? CanBeCancelled { get; set; }

            public OrderConflictsResponse()
            {
                SeatIds = new List<int>();
            }
        }
    }
}
