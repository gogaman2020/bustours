using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Bus: BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        [IgnoreField]
        public List<Table> Tables { get; set; }

        public Bus()
        {
            Tables = new List<Table>();
        }
    }
}
