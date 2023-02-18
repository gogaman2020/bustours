using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class ObjectPosition: BaseEntity
    {
        public short X { get; set; }

        public short Y { get; set; }

        public List<ObjectPosition> Childs { get; set; }
    }
}
