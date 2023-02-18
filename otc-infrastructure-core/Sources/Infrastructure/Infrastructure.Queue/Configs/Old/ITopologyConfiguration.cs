using RabbitMQ.Client;

namespace Infrastructure.Queue.Configs.Old
{
    public interface ITopologyConfiguration
    {
        void Configure(IModel model);
    }
}
