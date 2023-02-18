namespace BusTour.Domain.Models.Responses
{
    public class CalculationCostTourResponse
    {
        public decimal TourPrice { get; set; }

        public decimal VAT { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
