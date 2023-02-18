using BusTour.Domain.Enums;

namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - переход на другое правило.
    /// </summary>
    public class RedirectAction : BaseRuleAction
    {
        /// <summary>
        /// Тип правила для стола на двоих для перехода.
        /// </summary>
        public RuleTypesTwoSeats? RedirectRuleTypesTwoSeats { get; set; }

        /// <summary>
        /// Тип правила для стола на четверых для перехода.
        /// </summary>
        public RuleTypesFourSeats? RedirectRuleTypesFourSeats { get; set; }
    }
}
