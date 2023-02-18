using Infrastructure.Common.Configs;

namespace BusTour.Scheduler.Options
{
    [Config]
    public class ClientsConfig
    { 
        public string BusTourApiUrl { get; set; }
    }
}
