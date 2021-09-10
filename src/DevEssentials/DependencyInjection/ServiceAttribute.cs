using System;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute()
        {
        }

        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type? ServiceType { get; set; }

        public virtual ServiceLifetime Lifetime => ServiceLifetime.Transient;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedServiceAttribute : ServiceAttribute
    {
        public override ServiceLifetime Lifetime => ServiceLifetime.Scoped;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonServiceAttribute : ServiceAttribute
    {
        public override ServiceLifetime Lifetime => ServiceLifetime.Singleton;
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceContractAttribute : Attribute
    {
        public ServiceContractAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class DisallowAutomaticRegistrationAttribute : Attribute
    {
    }

}
