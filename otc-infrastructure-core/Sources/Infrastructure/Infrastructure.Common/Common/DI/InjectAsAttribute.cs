using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.DI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectAsAttribute : Attribute
    {
        public InjectAsAttribute(ServiceLifetime lifetime, params Type[] interfaces)
        {
            Lifetime = lifetime;
            Interfaces = interfaces;
        }

        public ServiceLifetime Lifetime { get; }
        public Type[] Interfaces { get;  }
    }

    public class InjectAsTransient : InjectAsAttribute
    {
        public InjectAsTransient(params Type[] interfaces) : base(ServiceLifetime.Transient, interfaces)
        {
        }
    }

    public class InjectAsScoped : InjectAsAttribute
    {
        public InjectAsScoped(params Type[] interfaces) : base(ServiceLifetime.Scoped, interfaces)
        {
        }
    }

    public class InjectAsSingleton : InjectAsAttribute
    {
        public InjectAsSingleton(params Type[] interfaces) : base(ServiceLifetime.Singleton, interfaces)
        {
        }
    }
}