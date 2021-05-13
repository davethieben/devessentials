using System;
using Essentials;
using Essentials.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSingletons(this IServiceCollection services, params object[] instances)
        {
            foreach (var instance in instances)
            {
                if (instance == null)
                    throw new ArgumentNullException("Singleton instances cannot be null");

                services.AddSingleton(instance.GetType(), instance);
            }
        }

        public static void Remove<TService>(this IServiceCollection services)
        {
            services.Remove(descriptor => descriptor.ServiceType.Is<TService>());
        }

    }
}