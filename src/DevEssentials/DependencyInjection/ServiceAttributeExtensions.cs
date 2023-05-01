using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials
{
    public static class ServiceAttributeExtensions
    {
        public static IServiceCollection AddAttributeServices(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            var assemblies = !assembliesToScan.IsNullOrEmpty() ? assembliesToScan.ToList() : new List<Assembly> { typeof(ServiceAttribute).Assembly };

            var contractTypes = assemblies.SelectMany(asm => asm.GetTypesWith<ServiceContractAttribute>());
            foreach (Type serviceType in contractTypes)
            {
                if (!serviceType.IsInterface)
                    throw new InvalidOperationException($"Types with a [ServiceContractAttribute] must be an interface: {serviceType}");

                AddInterface(services, serviceType);
            }

            var serviceTypes = assemblies.SelectMany(asm => asm.GetTypesWith<ServiceAttribute>());
            foreach (Type serviceType in serviceTypes)
            {
                if (!serviceType.IsClass)
                    throw new InvalidOperationException($"Types with a [ServiceAttribute] must be a class: {serviceType}");

                AddImplementation(services, serviceType);
            }

            return services;
        }

        private static void AddInterface(IServiceCollection services, Type serviceType)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<ServiceContractAttribute>();
            if (serviceAttribute.ServiceType == null)
                throw new InvalidOperationException($"[ServiceContractAttribute] must have a ServiceType specified");

            services.Add(ServiceDescriptor.Describe(serviceType, serviceAttribute.ServiceType, serviceAttribute.Lifetime));
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
                if (interfaces.Any())
                {
                    foreach (var serviceInterface in interfaces)
                    {
                        if (serviceInterface.IsDefined(typeof(DisallowAutomaticRegistrationAttribute)))
                            continue;

                        services.Add(ServiceDescriptor.Describe(serviceInterface, serviceType, serviceAttribute.Lifetime));
                    }
                }
                else
                {
                    services.Add(ServiceDescriptor.Describe(serviceType, serviceType, serviceAttribute.Lifetime));
                }
            }
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