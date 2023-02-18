namespace Infrastructure.Security.SecurityTokens
{
    public class SecurityTokenClientResponse
    {
        /// <summary>
        /// Признак успешности выполнения запроса.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Токен.
        /// </summary>
        public string SecurityToken { get; set; }

        /// <summary>
        /// Токен.
        /// </summary>
        public string SecurityTokenV2 { get; set; }
    }
}