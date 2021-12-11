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

            var mock = mockDescriptor.ImplementationInstance as Mock<TService>;
            if (mock == null)
                throw new ApplicationException($"Mock could not be created or registered: {typeof(Mock<TService>)}");

            mockSetup(mock);
            return this;
        }

        public ServiceFactorySetup AddImplementation<TImpl>()
            where TImpl : class
        {
            this.Remove<TImpl>();

            // extension for MVC:`
            //if (typeof(System.Web.Mvc.Controller).IsAssignableFrom(typeof(TImpl)))
            //{
            //    this.AddController(typeof(TImpl));
            //}
            //else
            {
                this.AddTransient<TImpl>();
            }

            this.AddDependencyMocks(typeof(TImpl));

            return this;
        }

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

    }
}
