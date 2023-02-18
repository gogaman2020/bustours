using BusTour.Domain.Entities;
using System.Collections.Generic;

namespace BusTour.Domain.Models
{
    public class MenuInfo
    {
        public List<Menu> Menus { get; set; }

        public List<Beverage> Beverages { get; set; }

        public List<Allergy> Allergies { get; set; }

        public List<Surprise> Surprises { get; set; }
    }
}
