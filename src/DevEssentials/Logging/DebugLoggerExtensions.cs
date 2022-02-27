using System;
using Essentials;
using Essentials.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public static class DebugLoggerExtensions
    {
        public static ILoggingBuilder AddBetterDebug(this ILoggingBuilder builder, Action<DebugLoggerOptions>? setup = null)
        {
            builder.Services.Remove(sd => sd.ImplementationType == typeof(Microsoft.Extensions.Logging.Debug.DebugLoggerProvider));
            builder.Services.AddSingleton<ILoggerProvider, DebugLoggerProvider>();

            if (setup != null)
                builder.Services.Configure<DebugLoggerOptions>(setup);

            return builder;
        }

    }
}
