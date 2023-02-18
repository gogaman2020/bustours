namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Базовый ответ.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Признак успешного выполнения запроса.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; set; }
    }
}
