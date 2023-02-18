namespace BusTour.Domain.Models.Order
{
    public class OrderClientModel
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsSigned { get; set; }
    }
}
