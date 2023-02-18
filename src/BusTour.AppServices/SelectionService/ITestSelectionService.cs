using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;

namespace BusTour.AppServices.SelectionService
{
    public interface ITestSelectionService
    {
        TestSelectionResult Select(TestBusModel busModel, SelectionInfo selectionInfo, bool isLockMode = false);

        TestResponseDebugInfo GetRulesDebugInfo(TestBusModel busModel, SelectionInfo selectionInfo);
    }
}
