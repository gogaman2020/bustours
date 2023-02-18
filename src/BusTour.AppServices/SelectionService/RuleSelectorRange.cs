namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Диапазон выбранного количества гостей для применения правил подбора лучшего правила. 
    /// </summary>
    public class RuleSelectorRange
    { 
        /// <summary>
        /// Минимальное количество гостей.
        /// </summary>
        public int FromGuestCount { get; set; }

        /// <summary>
        /// Максимальное количество гостей.
        /// </summary>
        public int? ToGuestCount { get; set; }

        /// <summary>
        /// Объект для подбора лучшего правила.
        /// </summary>
        public RuleSelector Selector { get; set; }
    }
}
