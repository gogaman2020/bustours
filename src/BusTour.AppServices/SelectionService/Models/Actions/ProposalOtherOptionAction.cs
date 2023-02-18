namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - предлагаем выбрать другую опцию или иной день или время
    /// </summary>
    public class OtherOptionAction : BaseRuleAction
    {
        /// <inheritdoc/>
        public override bool NoProposals => true;
    }
}
