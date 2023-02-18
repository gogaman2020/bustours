using System;
using System.Collections.Generic;

namespace Infrastructure.Health.Config
{
    public static class HealthCheckConfig
    {
        public static HealthCheckConfigClients Clients => HealthCheckConfigClients.Instance;
        public static HealthCheckConfigDb Db => HealthCheckConfigDb.Instance;
        public static HealthCheckConfigRabbitMq RabbitMq => HealthCheckConfigRabbitMq.Instance;
    }

    public abstract class HealthCheckConfigT<T>
        where T : class, new()
    {
        public static readonly T Instance = new T();

        public bool Disabled { get; set; } = false;
    }

    public abstract class HealthCheckConfigNamesT<T> : HealthCheckConfigT<T>
        where T : class, new()
    {
        protected HashSet<string> _configNames = new HashSet<string>();

        public IReadOnlyCollection<string> ConfigNames => _configNames;

        public void AddConfigName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException($"{nameof(name)}:{name}");
            }

            _configNames.Add(name);
        }

        public void ClearNames()
        {
            _configNames.Clear();
        }

        public void DropDefaultName()
        {
            _configNames.Clear();
        }
    }

    public class HealthCheckConfigClients : HealthCheckConfigT<HealthCheckConfigClients>
    {
    }

    public class HealthCheckConfigDb : HealthCheckConfigNamesT<HealthCheckConfigDb>
    {
        public HealthCheckConfigDb()
        {
            _configNames.Add("DbConfig");
        }
    }

    public class HealthCheckConfigRabbitMq : HealthCheckConfigNamesT<HealthCheckConfigRabbitMq>
    {
        public HealthCheckConfigRabbitMq()
        {
            _configNames.Add("RabbitMqConfig");
        }
    }
}