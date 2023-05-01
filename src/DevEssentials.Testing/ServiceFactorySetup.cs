using System;
using System.Linq;
using Essentials;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DevEssentials.Testing
{
    /// <summary>
    /// helper class for setting up <see cref="ServiceFactory"/>
    /// </summary>
    public class ServiceFactorySetup : ServiceCollection
    {
        public ServiceFactorySetup()
        {
        }

        /// <summary>
        /// configure and set up the <see cref="Mock{TService}"/> thats been added to the container
        /// </summary>
        /// <typeparam name="TService">service Type to configure the Mock</typeparam>
        /// <param name="mockSetup">delegate to perform the configuration</param>
        /// <returns>itself</returns>
        public ServiceFactorySetup Setup<TService>(Action<Mock<TService>> mockSetup)
            where TService : class
        {
            var mockDescriptor = this.FirstOrDefault(sd => sd.ServiceType == typeof(Mock<TService>));
            if (mockDescriptor == null)
            {
                this.AddMock<TService>();

                mockDescriptor = this.FirstOrDefault(sd => sd.ServiceType == typeof(Mock<TService>));
            }

            if (mockDescriptor?.ImplementationInstance is Mock<TService> mock)
                mockSetup(mock);

            return this;
        }

        /// <summary>
        /// Adds a concrete implementation of {<typeparamref name="TImpl"/>} to the container only
        /// resolvable by the Type itself.
        /// </summary>
        /// <typeparam name="TImpl">Type of the Implementation</typeparam>
        /// <param name="singleton">Optional instance of {<typeparamref name="TImpl"/> to be used as a Singleton</param>
        /// <returns>itself</returns>
        public ServiceFactorySetup AddImplementation<TImpl>(TImpl? singleton = null)
            where TImpl : class
        {
            this.Remove<TImpl>();

            // extension for AspNetCore:
            //if (typeof(Controller).IsAssignableFrom(typeof(TImpl)))
            //{
            //    this.AddController(typeof(TImpl));
            //}
            if (singleton != null)
            {
                this.AddSingleton<TImpl>(singleton);
            }
            else
            {
                this.AddTransient<TImpl>();
                this.AddDependencyMocks(typeof(TImpl));
            }

            return this;
        }

        /// <summary>
        /// Adds a concrete implementation of {<typeparamref name="TService"/>} to the container, and
        /// removes any existing Mocks of {<typeparamref name="TService"/>}.
        /// </summary>
        /// <typeparam name="TService">Type of the Service</typeparam>
        /// <typeparam name="TImpl">Type of the Implementation</typeparam>
        /// <returns></returns>
        public ServiceFactorySetup AddImplementation<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            this.Remove<TService>();
            this.Remove<Mock<TService>>();

            this.AddTransient<TService, TImpl>();
            this.AddDependencyMocks(typeof(TImpl));

            return this;
        }

        /// <summary>
        /// Adds an additional implementation of {<typeparamref name="TService"/>} to the container.
        /// </summary>
        /// <typeparam name="TService">Type of the Service</typeparam>
        /// <typeparam name="TImpl">Type of the Implementation</typeparam>
        /// <returns>itself</returns>
        public ServiceFactorySetup AddEnumerable<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            this.Remove<Mock<TService>>();
            this.AddTransient<TService, TImpl>();
            this.AddDependencyMocks(typeof(TImpl));
            return this;
        }

    }
}
