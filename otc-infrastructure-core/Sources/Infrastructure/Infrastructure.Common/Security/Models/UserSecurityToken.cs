using System;

namespace Infrastructure.Security.Models
{
    /// <summary>
    /// Токен безопасности пользователя.
    /// </summary>
    [Serializable]
    public class UserSecurityToken : ISecurityToken
    {
        #region Constructors

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="session">Сессия.</param>
        /// <param name="userInfo">Информация о пользователе.</param>
        public UserSecurityToken(Session session, UserInfo userInfo)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (userInfo == null)
                throw new ArgumentNullException("userInfo");

            Session = session;
            UserInfo = userInfo;
        }

        #endregion Constructors

        #region Члены IUserSecurityToken

        public UserInfo UserInfo { get; set; }

        #endregion Члены IUserSecurityToken

        #region Члены ISecurityToken

        public Session Session { get; set; }

        public bool IsUserSecurityToken
        {
            get { return true; }
        }

        public bool IsServiceSecurityToken
        {
            get { return false; }
        }

        #endregion Члены ISecurityToken
    }
}
