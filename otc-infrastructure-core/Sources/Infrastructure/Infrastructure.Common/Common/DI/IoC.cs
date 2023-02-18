using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.DI
{
    public static class IoC
    {
        private class LocalScopeDisposable : IDisposable
        {
            private bool _disposed = false;

            public LocalScopeDisposable(Action action)
            {
                Action = action;
            }

            public Action Action { get; }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Action();
                }
            }
        }

        private class Data
        {
            public IServiceProvider Provider { get; set; }
        }

        private static IServiceProvider _serviceProvider;
        private static readonly AsyncLocal<Data> _serviceProviderLocal = new AsyncLocal<Data>();

        public static void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IDisposable InitScopeProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                if (_serviceProviderLocal.Value != null)
                {
                    _serviceProviderLocal.Value.Provider = null;
                }

                return new LocalScopeDisposable(() => { });
            }

            _serviceProviderLocal.Value = new Data {Provider = serviceProvider};
            return new LocalScopeDisposable(() => { InitScopeProvider(null); });
        }

        private static IServiceProvider Provider => _serviceProviderLocal.Value?.Provider ?? _serviceProvider;

        public static bool IsInScope => _serviceProviderLocal.Value?.Provider != null;

        [Obsolete]
        public static T Resolve<T>(bool force = false)
        {
            return force
                ? Provider.GetRequiredService<T>()
                : Provider.GetService<T>();
        }

        [Obsolete]
        public static object Resolve(Type type, bool force = false)
        {
            return force
                ? Provider.GetRequiredService(type)
                : Provider.GetService(type);
        }

        public static T GetService<T>()
        {
            return Provider.GetService<T>();
        }

        public static T GetService<T>(int key)
        {
            return Provider.GetService<T>(key);
        }

        public static object GetService(Type type)
        {
            return Provider.GetService(type);
        }

        public static T GetRequiredService<T>()
        {
            return Provider.GetRequiredService<T>();
        }

        public static object GetRequiredService(Type type)
        {
            return Provider.GetRequiredService(type);
        }

        public static Task RunInNewScopeAsync(Action action)
        {
            return RunInNewScopeAsync(sp =>
            {
                action();
                return Task.CompletedTask;
            });
        }

        public static Task RunInNewScopeAsync(Func<Task> func, IServiceProvider parent = null)
        {
            return RunInNewScopeAsync(async sp => { await func(); });
        }

        public static async Task RunInNewScopeAsync(Func<IServiceProvider, Task> func, IServiceProvider parent = null)
        {
            using var scope = (parent ?? Provider).CreateScope();
            using var iocscope = InitScopeProvider(scope.ServiceProvider);
            await func(scope.ServiceProvider);
        }
    }
}