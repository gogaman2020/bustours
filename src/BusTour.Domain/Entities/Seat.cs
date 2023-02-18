using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class Seat : BaseEntity
    {
        public string Name { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        public bool IsForward { get; set; }

        public bool IsBackward { get; set; }

        public decimal Price { get; set; }

        public int TableId { get; set; }

        public int Rotate { get; set; }

        public int ScaleX { get; set; }

        public int ScaleY { get; set; }

        public SeatType Type { get; set; }

        [IgnoreField]
        public Table Table { get; set; }
    }
    public enum SeatType
    {
        Default = 1,
        Side = 2,
        Long = 3,
        Double = 4,

        /// <summary>
        /// Место для инвалидов.
        /// </summary>
        Disabled = 5
    }
}
