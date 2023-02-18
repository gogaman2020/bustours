using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Menu: BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        public decimal Price { get; set; }

        public string ImgPath { get; set; }

        public decimal Volume { get; set; }

        public Dictionary<string, string> Unit { get; set; }

        public MenuType MenuType { get; set; }
    }
}
