using Infrastructure.Db.Common;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Table : BaseEntity
    {
        public byte Number { get; set; }

        public bool IsAvailable { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        public byte XSize { get; set; }

        public byte YSize { get; set; }

        public bool IsLeft { get; set; }

        public bool IsRight { get; set; }

        public byte Floor { get; set; }

        public decimal Price { get; set; }

        public TableCategory Category { get; set; }

        public List<Seat> Seats { get; set; }

        //todo: Нужно переделать признак
        [IgnoreField]
        public bool IsVip => Category.Id > 1;

        public Table()
        {
            Seats = new List<Seat>();
        }
    }
}
