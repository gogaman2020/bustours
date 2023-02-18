namespace Infrastructure.Security.Models
{
    /// <summary>
    /// Информация о пользователе.
    /// </summary>
    public class UserInfoNotSerializable
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор сотрудника.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Идентификатор организации.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Номер приложения.
        /// </summary>
        public int PlatformId { get; set; }
    }
}
