using BusTour.AppServices.TourOrderProcess.Steps;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Process;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourOrderProcess
{
    [InjectAsScoped]
    public class TourOrderProcess : ProcessBase<Order, OrderProcessState>, ITourOrderProcess
    {
        public TourOrderProcess(OrderProcessRepository repository, IProcessAudit audit)
            : base(repository, audit)
        {
        }

        public override string StartStepName => nameof(TourOrderDraftStep);

        protected override int? ObjectId => Context?.Id;

        protected override bool IsUsing(Order use, int id)
        {
            return use?.Id == id;
        }

        protected override Task<Order> LoadAsync(int id)
        {
            return IoC.GetRequiredService<IOrderRepository>()
                .GetAsync(id);
        }
    }
}
