using Infrastructure.Common.DI;

namespace Infrastructure.Common.Configs
{
    public interface IConfig<T>
        where T : class, new()
    {
        T Value { get; }
    }

    [InjectAsSingleton]
    public class ConfigImpl<T> : IConfig<T>
        where T : class, new()
    {
        public T Value => Config.Get<T>();
    }
}