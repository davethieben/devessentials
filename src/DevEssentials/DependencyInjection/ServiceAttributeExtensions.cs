using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Essentials;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceAttributeExtensions
    {
        public static IServiceCollection AddAttributeServices(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            var assemblies = !assembliesToScan.IsNullOrEmpty() ? assembliesToScan.ToList() : new List<Assembly>();
            assemblies.Add(typeof(ServiceAttribute).Assembly);

            var types = assemblies.SelectMany(asm => asm.GetTypesWith<ServiceAttribute>());
            foreach (Type serviceType in types)
            {
                var serviceAttribute = serviceType.GetCustomAttribute<ServiceAttribute>();
                if (serviceType.IsInterface)
                {
                    AddInterface(services, serviceType, assemblies);
                }
                else if (serviceType.IsClass)
                {
                    AddImplementation(services, serviceType);
                }
                else
                {
                    throw new InvalidOperationException($"Types with a [ServiceAttribute] must be a class or interface: {serviceType}");
                }
            }

            return services;
        }

        private static void AddInterface(IServiceCollection services, Type serviceType, IEnumerable<Assembly> sources)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute.ServiceType != null)
            {
                services.Add(ServiceDescriptor.Describe(serviceType, serviceAttribute.ServiceType, serviceAttribute.Lifetime));
            }
            else
            {
                var implementations = GetAllAssignable(serviceType);
                foreach (var impl in implementations)
                {
                    services.Add(ServiceDescriptor.Describe(serviceType, impl, serviceAttribute.Lifetime));
                }
            }

            IEnumerable<Type> GetAllAssignable(Type target) =>
                sources.SelectMany(assembly =>
                    assembly.GetExportedTypes()
                        .Where(t => target.IsAssignableFrom(t)));
        }

        private static void AddImplementation(IServiceCollection services, Type serviceType)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute.ServiceType != null)
            {
                services.Add(ServiceDescriptor.Describe(serviceAttribute.ServiceType, serviceType, serviceAttribute.Lifetime));
            }
            else
            {
                var interfaces = serviceType.GetInterfaces();
                foreach (var serviceInterface in interfaces)
                {
                    services.Add(ServiceDescriptor.Describe(serviceInterface, serviceType, serviceAttribute.Lifetime));
                }
            }

            services.AddSelf(serviceType, serviceAttribute.Lifetime);
        }

        public static IServiceCollection AddSelf<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services.AddSelf(typeof(T), lifetime);
        }

        public static IServiceCollection AddSelf(this IServiceCollection services, Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            services.Add(ServiceDescriptor.Describe(serviceType, serviceType, lifetime));
            return services;
        }

        public static IEnumerable<Type> GetTypesWith<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute =>
            assembly.GetExportedTypes().Where(t => t.IsDefined(typeof(TAttribute)));



    }
}