namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о госте для вкладки "Меню".
    /// </summary>
    public class ResponseSelectionMenuGuestInfo
    {
        /// <summary>
        /// ИД места.
        /// </summary>
        public int SeatId { get; set; }

        /// <summary>
        /// Наименованием места с учётом стола.
        /// </summary>
        public string SeatFullName { get; set; }
    }
}
