namespace BusTour.Domain.Models.Order
{
    public class OrderSeatModel
    {
        public int SeatId { get; set; }
        public int? MenuId { get; set; }
        public int? BeverageId { get; set; }
        public int? AllergyId { get; set; }
        public string OtherAllergy { get; set; }
        public bool? IsEmpty { get; set; }
        public bool? GuestHasCome { get; set; }
        public bool? HasMenuIssued { get; set; }
        public bool? HasBeverageIssued { get; set; }
    }
}
