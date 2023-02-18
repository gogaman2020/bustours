using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class City : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        public int CountryId { get; set; }

        [IgnoreField]
        public Country Country { get; set; }
    }
}
