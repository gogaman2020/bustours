using System.Threading.Tasks;

namespace Infrastructure.Queue.Publishers
{
    public interface IPublisher<T>
        where T : class
    {
        Task PublishAsync(T message);
    }
}