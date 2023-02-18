using BusTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Order
{
    public class OrderExtras
    {
        public List<OrderMenu> Menus { get; set; }
        public List<OrderBeverage> Beverages { get; set; }
    }
}
