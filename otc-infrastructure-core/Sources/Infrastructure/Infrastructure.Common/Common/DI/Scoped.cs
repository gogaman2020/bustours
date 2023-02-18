using System.Threading;

namespace Infrastructure.Common.DI
{
    [InjectAsSingleton]
    public class Scoped<T>
    {
        private static readonly AsyncLocal<T> _service = new AsyncLocal<T>();
        public virtual T Service => _service.Value ?? (_service.Value = IoC.GetService<T>());
        public virtual T ServiceRequired => _service.Value ?? (_service.Value = IoC.GetRequiredService<T>());
    }

    [InjectAsSingleton]
    public class ScopedWithFallback<T, TScoped> : Scoped<T>
        where TScoped : T
    {
        private static readonly AsyncLocal<T> _service = new AsyncLocal<T>();

        public override T Service => _service.Value ??
                                     (_service.Value = IoC.IsInScope
                                         ? IoC.GetService<TScoped>()
                                         : IoC.GetService<T>());

        public override T ServiceRequired => _service.Value ??
                                             (_service.Value = IoC.IsInScope
                                                 ? IoC.GetRequiredService<TScoped>()
                                                 : IoC.GetRequiredService<T>());
    }
}