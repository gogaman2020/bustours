namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Ответ на действие с заказом.
    /// </summary>
    public class OrderResponse : BaseResponse
    { 
        /// <summary>
        /// ИД заказа
        /// </summary>
        public int OrderId { get; set; }


        /// <summary>
        /// Хэш заказа
        /// </summary>
        public string Hash { get; set; }
    }
}
