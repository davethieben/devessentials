using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Essentials.Configuration
{
    public class ConfigurationOptionsBinder<TOptions> : IConfigureOptions<TOptions>
        where TOptions : class
    {
        private readonly IConfiguration _configuration;

        public ConfigurationOptionsBinder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(TOptions options)
        {
            ConfigurationBinder.Bind(_configuration, options);
        }

    }
}
