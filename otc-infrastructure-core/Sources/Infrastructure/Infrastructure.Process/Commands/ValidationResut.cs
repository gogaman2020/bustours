namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Результат валидации шага
    /// </summary>
    public sealed class ValidationResut
    {
        /// <summary>
        /// Признак валидности
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Причина невалидности
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Создает валидный результат
        /// </summary>
        /// <returns></returns>
        public static ValidationResut Valid()
        {
            return new ValidationResut { IsValid = true };
        }

        /// <summary>
        /// Создает невалидный результат с указанием причины
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static ValidationResut InValid(string reason)
        {
            return new ValidationResut { Reason = reason };
        }
    }
}
