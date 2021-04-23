using System;
using Essentials.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Essentials.Extensions
{
    public static class OptionsExtensions
    {
        public static ConfigureOptionsBuilder<TOptions> Configure<TOptions>(this IServiceCollection services)
            where TOptions : class
        {
            var builder = new ConfigureOptionsBuilder<TOptions>(services);
            return builder;
        }

        public static ConfigureOptionsBuilder<TOptions> Configure<TOptions>(this IServiceCollection services, string name)
            where TOptions : class
        {
            var builder = new ConfigureOptionsBuilder<TOptions>(services, name);
            return builder;
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Type typeForName, Action<TOptions> configureOptions) where TOptions : class
            => services.Configure(typeForName.IsRequired().GetDisplayName(), configureOptions);

        public static ConfigureOptionsBuilder<TOptions> Configure<TOptions>(this IServiceCollection services, Type typeForName) where TOptions : class
            => services.Configure<TOptions>(typeForName.IsRequired().GetDisplayName());

        public class ConfigureOptionsBuilder<TOptions>
            where TOptions : class
        {
            private readonly IServiceCollection _services;

            public ConfigureOptionsBuilder(IServiceCollection services, string? name = null)
            {
                _services = services.IsRequired();
                Name = name ?? Options.DefaultName;
            }

            public string Name { get; }

            public void With<TService1>(Action<TOptions, TService1> action)
                where TService1 : notnull
            {
                if (Name.HasValue())
                {
                    _services.AddSingleton<IConfigureOptions<TOptions>>(sp =>
                    {
                        return new ConfigureNamedOptions<TOptions>(Name, options =>
                        {
                            var service1 = sp.GetRequiredService<TService1>();
                            action.Invoke(options, service1);
                        });
                    });
                }
                else
                {
                    _services.AddSingleton<IConfigureOptions<TOptions>>(sp =>
                    {
                        return new ConfigureOptions<TOptions>(options =>
                        {
                            var service1 = sp.GetRequiredService<TService1>();
                            action.Invoke(options, service1);
                        });
                    });
                }
            }

        }
    }
}
