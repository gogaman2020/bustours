using System.Threading.Tasks;

namespace Infrastructure.Queue.Subscribers
{
    public interface ISubscriber<T>
        where T : class
    {
        Task ConsumeAsync(T message);
    }
}
