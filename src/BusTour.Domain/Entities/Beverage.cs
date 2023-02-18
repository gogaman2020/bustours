using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Beverage: BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        public decimal Price { get; set; }

        public string ImgPath { get; set; }

        public decimal Volume { get; set; }

        public Dictionary<string, string> Unit { get; set; }

        public decimal? AlcoholByVolume { get; set; }

        public bool IsHot { get; set; }

        public BeverageGroup Group { get; set; }

        public WineType WineType { get; set; }
    }
}
