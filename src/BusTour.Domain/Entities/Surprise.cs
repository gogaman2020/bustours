using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Surprise : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        public decimal Price { get; set; }

        public string ImgPath { get; set; }
    }
}
