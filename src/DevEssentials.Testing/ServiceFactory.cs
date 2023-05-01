using System;
using System.Linq;
using System.Reflection;
using Essentials;
using Essentials.Reflection;
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
        public static ServiceFactorySetup GenerateFor<T>(Action<IServiceCollection>? setup = null)
            where T : class
        {
            var services = new ServiceFactorySetup();
            setup?.Invoke(services);

            services.AddLogging();
            services.AddOptions();

            return services.AddImplementation<T>();
        }

        public static ServiceFactorySetup AddDependencyMocks(this ServiceFactorySetup services, Type implType)
        {
            var constructors = implType.GetConstructors().OrderBy(c => c.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (!services.Contains(parameter.ParameterType)
                        && !parameter.ParameterType.IsGenericEnumerable()
                        && (parameter.ParameterType.IsInterface || parameter.ParameterType.IsAbstract))
                    {
                        services.AddMock(parameter.ParameterType);
                    }
                }

                break;
            }

            return services;
        }

        public static ServiceFactorySetup AddMock<T>(this ServiceFactorySetup services, Action<Mock<T>>? setup = null)
            where T : class
        {
            services.IsRequired();

            var mockDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(Mock<T>));
            if (mockDescriptor == null)
            {
                services.AddMock(typeof(T));
            }

            var mock = services.FirstOrDefault(sd => sd.ServiceType == typeof(Mock<T>))?.ImplementationInstance as Mock<T>;
            if (mock != null && setup != null)
                setup.Invoke(mock);

            return services;
        }

        public static ServiceFactorySetup AddMock(this ServiceFactorySetup services, Type serviceType)
        {
            var mockType = typeof(Mock<>);
            var implMockType = mockType.MakeGenericType(serviceType);
            var mockConstruct = implMockType.GetConstructor(new Type[] { });
            var mock = mockConstruct.Invoke(null) as Mock;
            if (mock == null)
                throw new InvalidOperationException($"Cannot find default constructor for Type '{implMockType}'");

            services.AddSingleton(implMockType, mock); // add Mock<TService>

            services.RemoveService(serviceType);
            services.AddTransient(serviceType, sp => mock.Object); // add TService

            return services;
        }

    }
}
