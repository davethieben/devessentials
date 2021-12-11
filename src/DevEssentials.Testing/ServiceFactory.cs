using System;
using System.Linq;
using Essentials;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DevEssentials.Testing
{
    /// <summary>
    /// ServiceFactory is a test helper for creating and configuring a dependency injection
    /// container to use when testing service classes with lots of dependencies. 
    /// </summary>
    public static class ServiceFactory
    {
        public static ServiceFactorySetup Create(params object[] singletonInstances)
        {
            var services = new ServiceFactorySetup();

            if (!singletonInstances.IsNullOrEmpty())
            {
                foreach (var instance in singletonInstances)
                {
                    if (instance != null)
                        services.AddSingleton(instance.GetType(), instance);
                }
            }

            return services;
        }

        public static ServiceFactorySetup GenerateFor<T>(params object[] singletonInstances)
            where T : class
        {
            return Create(singletonInstances)
                .AddImplementation<T>();
        }

        public static IServiceCollection AddDependencyMocks(this IServiceCollection services, Type implType)
        {
            var constructors = implType.GetConstructors().OrderBy(c => c.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                System.Reflection.ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Any(p => !p.ParameterType.IsInterface))
                    continue;

                foreach (var parameter in parameters)
                {
                    if (!services.Contains(parameter.ParameterType))
                    {
                        services.AddMock(parameter.ParameterType);
                    }
                }

                break;
            }

            return services;
        }

        public static IServiceCollection AddMock<T>(this IServiceCollection services, Action<Mock<T>>? setup = null)
            where T : class
        {
            services.IsRequired();
            services.AddMock(typeof(T));

            var mock = services.FirstOrDefault(sd => sd.ServiceType == typeof(Mock<T>))?.ImplementationInstance as Mock<T>;
            if (mock != null && setup != null)
                setup.Invoke(mock);

            return services;
        }

        public static IServiceCollection AddMock(this IServiceCollection services, Type serviceType)
        {
            var mockType = typeof(Mock<>);
            var implMockType = mockType.MakeGenericType(serviceType);
            var mockConstruct = implMockType.GetConstructor(new Type[] { });
            var mock = mockConstruct.Invoke(null) as Mock;

            services.AddSingleton(implMockType, mock); // add Mock<TService>
            services.AddTransient(serviceType, sp => mock.Object); // add TService

            return services;
        }
    
    }
}
