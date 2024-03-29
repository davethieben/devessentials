﻿using Microsoft.Extensions.DependencyInjection;

namespace Essentials
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRequestScoped<TService, TImpl>(this IServiceCollection services)
            where TService : class
            where TImpl : TService
        {
            services.IsRequired();
            services.Add(new RequestServiceDescriptor(typeof(TService), typeof(TImpl), ServiceLifetime.Transient));

            return services;
        }

        public static IServiceCollection TryAddRequestScoped<TService, TImpl>(this IServiceCollection services)
            where TService : class
            where TImpl : TService
        {
            services.IsRequired();

            if (!services.Contains(typeof(TService)))
                services.Add(new RequestServiceDescriptor(typeof(TService), typeof(TImpl), ServiceLifetime.Transient));

            return services;
        }

        public static void TryAddImplementation<TService, TImplementation>(this IServiceCollection services)
            where TService : class where TImplementation : class, TService
        {
            services.IsRequired();

            if (!services.ContainsImplementation(typeof(TImplementation)))
            {
                services.AddTransient<TService, TImplementation>();
            }
        }

        public static bool Contains<TService>(this IServiceCollection services) => services.Contains(typeof(TService));

        public static bool Contains(this IServiceCollection services, Type serviceType)
        {
            if (services.IsNullOrEmpty())
                return false;

            serviceType.IsRequired();
            return services.Any(sd => sd.ServiceType == serviceType)
                || (serviceType.IsGenericType && services.Any(sd => sd.ServiceType == serviceType.GetGenericTypeDefinition()));
        }

        public static bool ContainsImplementation(this IServiceCollection services, Type implType)
        {
            if (services.IsNullOrEmpty())
                return false;

            implType.IsRequired();
            return services.Any(sd => sd.ImplementationType == implType);
        }

        public static void AddSingletons(this IServiceCollection services, params object[] instances)
        {
            foreach (var instance in instances)
            {
                if (instance == null)
                    throw new ArgumentNullException("Singleton instances cannot be null");

                services.AddSingleton(instance.GetType(), instance);
            }
        }

        public static void Remove<TService>(this IServiceCollection services) => services.RemoveService(typeof(TService));

        public static void RemoveService(this IServiceCollection services, Type serviceType)
        {
            services.IsRequired();
            serviceType.IsRequired();

            var descriptors = services.Where(sd => sd.ServiceType == serviceType).ToArray();
            if (!descriptors.IsNullOrEmpty())
            {
                foreach (var descriptor in descriptors)
                    services.Remove(descriptor);
            }
        }

    }

    public class RequestServiceDescriptor : ServiceDescriptor
    {
        public RequestServiceDescriptor(Type serviceType, object instance)
            : base(serviceType, instance) { }

        public RequestServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
            : base(serviceType, implementationType, lifetime) { }

        public RequestServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
            : base(serviceType, factory, lifetime) { }
    }

}
