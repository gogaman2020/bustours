using System;
using System.Threading.Tasks;

namespace Infrastructure.Mediator.Context
{
    public interface IContextFactory
    {
        Task UseContextAsync(Func<Task> inContext);
        Task UseContextAsync(Func<Task> inContext, Context context);
    }
}
