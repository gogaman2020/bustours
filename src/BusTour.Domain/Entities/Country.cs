using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Country : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }
    }
}
