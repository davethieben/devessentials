using System;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        public Type ServiceType { get; set; } = default!;
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }
}
