using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Essentials.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Essentials
{
    public static class LoggerExtensions
    {
        public static void LogDebugObject<T>(this ILogger logger, T? target, string? name = null, int depth = 10) =>
            logger.LogObject(LogLevel.Debug, target, name, depth);

        public static void LogObject<T>(this ILogger logger, LogLevel logLevel, T? target, string? name = null, int maxDepth = 10)
        {
            logger.IsRequired();

            if (logger.IsEnabled(logLevel))
            {
                name = name ?? target?.GetType().FullName ?? typeof(T).FullName;
                string json = JsonConvert.SerializeObject(target, Formatting.Indented,
                    new JsonSerializerSettings { MaxDepth = maxDepth, NullValueHandling = NullValueHandling.Ignore });

                logger.Log(logLevel, name + Environment.NewLine + json);
            }
        }

        public static string? GetCallingMethodName(this StackTrace stackTrace)
        {
            var frames = (stackTrace?.GetFrames()).EmptyIfNull();

            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();
                Type methodsType = method.DeclaringType ?? method.ReflectedType;

                if (methodsType != typeof(LoggerExtensions)
                    && methodsType?.Namespace.StartsWith("System") != true
                    && methodsType?.Namespace.StartsWith("Microsoft") != true)
                {
                    if (methodsType?.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() != null)
                    {
                        return methodsType.Name + "." + method.Name;

                    }

                    return method.Name;
                }
            }

            return null;
        }

        public static void LogMethodStart(this ILogger logger, LogLevel logLevel = LogLevel.Information)
        {
            logger.IsRequired()
                .Log(logLevel, $"{{{new StackTrace().GetCallingMethodName()}}} Start");
        }

        public static void LogMethodEnd(this ILogger logger, LogLevel logLevel = LogLevel.Information)
        {
            logger.IsRequired()
                .Log(logLevel, $"{{{new StackTrace().GetCallingMethodName()}}} End");
        }

    }
}

namespace Essentials.Logging
{
    public static class LoggerExtensions
    {
        public static ILoggerFactory CreateFactory(Action<ILoggingBuilder> setup)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(setup)
                .BuildServiceProvider();

            return serviceProvider.GetRequiredService<ILoggerFactory>();
        }

        public static ILogger CreateLogger(this ILoggerFactory factory, params Type[] names)
        {
            factory.IsRequired();
            Contract.Requires(names != null && names.Length >= 1, "Names must be provided");

            string fullname = string.Join(".",
                new[] { names.First().GetDisplayName() }
                .Concat(names.Skip(1)
                    .Select(t => t.Name)));

            return factory.CreateLogger(fullname);
        }

        public static ILogger CreateLogger<T1, T2>(this ILoggerFactory factory)
        {
            return factory.CreateLogger(typeof(T1), typeof(T2));
        }

        public static ILogger CreateLogger<T1, T2, T3>(this ILoggerFactory factory)
        {
            return factory.CreateLogger(typeof(T1), typeof(T2), typeof(T3));
        }

        public static ILogger CreateLogger<T1, T2, T3, T4>(this ILoggerFactory factory)
        {
            return factory.CreateLogger(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public static StreamLogger OpenStreamLogger(this Stream stream, string name)
        {
            stream.IsRequired();

            return new StreamLogger(name, stream);
        }

    }
}
