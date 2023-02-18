using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common.Plugins
{
    public interface IHostConfigPlugin
    {
        public void Add(IConfigurationBuilder config, string environmentName);
    }
}