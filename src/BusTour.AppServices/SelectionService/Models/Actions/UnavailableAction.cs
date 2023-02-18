namespace BusTour.AppServices.SelectionService.Models.Actions
{
    /// <summary>
    /// Правило - недостижимое состояние
    /// </summary>
    public class UnavailableAction : BaseRuleAction
    {
        /// <inheritdoc/>
        public override bool NoProposals => true;
    }
}
