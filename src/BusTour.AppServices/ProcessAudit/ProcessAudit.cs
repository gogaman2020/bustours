using Infrastructure.Common.DI;
using Infrastructure.Process;
using Infrastructure.Process.Args;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourProcess.ProcessAudit
{
    [InjectAsSingleton]
    public class ProcessAudit : IProcessAudit
    {
        public Task AuditAsync(int? objectId, StepCommandArgs commandArgs, string stepFrom, string stepTo = null, StepCommandArgs toArgs = null)
        {
            //stub implementation
            return Task.CompletedTask;
        }
    }
}
