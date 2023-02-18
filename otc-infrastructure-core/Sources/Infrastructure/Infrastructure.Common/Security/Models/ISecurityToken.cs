namespace Infrastructure.Security.Models
{
    public interface ISecurityToken
    {
        /// <summary>
        /// Текущая сессия.
        /// </summary>
        Session Session { get; }

        /// <summary>
        /// Возвращает значение, указывающее, является ли токен безопасности токеном пользователя.
        /// </summary>
        bool IsUserSecurityToken { get; }

        /// <summary>
        /// Возвращает значение, указывающее, является ли токен безопасности токеном сервиса.
        /// </summary>
        bool IsServiceSecurityToken { get; }

        /// <summary>
        /// Возвращает информацию о пользователе.
        /// </summary>
        UserInfo UserInfo { get; }
    }

    
}
