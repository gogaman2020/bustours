namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о выбранном месте для вкладки "Меню".
    /// </summary>
    public class ResponseSelectionOrderSeatInfo
    {
        /// <summary>
        /// ИД места.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Положение места по оси X.
        /// </summary>
        public short X { get; set; }

        /// <summary>
        /// Положение места по оси Y.
        /// </summary>
        public short Y { get; set; }

        /// <summary>
        /// Признак, что место выбрано.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
