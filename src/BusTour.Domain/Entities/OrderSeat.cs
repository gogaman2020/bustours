using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class OrderSeat: BaseEntity
    {
        public int? OrderId { get; set; }

        public int SeatId { get; set; }

        public int? MenuId { get; set; }

        public int? BeverageId { get; set; }

        public int? AllergyId { get; set; }

        public string OtherAllergy { get; set; }

        public bool? IsEmpty { get; set; }

        /// <summary>
        /// пришел ли гость
        /// </summary>
        public bool? GuestHasCome { get; set; }

        /// <summary>
        /// Выдано ли меню
        /// </summary>
        public bool? HasMenuIssued { get; set; }

        /// <summary>
        /// Выдан ли напиток
        /// </summary>
        public bool? HasBeverageIssued { get; set; }

        /// <summary>
        /// Цена места в заказе.
        /// </summary>
        public decimal? Price { get; set; }
    }
}
