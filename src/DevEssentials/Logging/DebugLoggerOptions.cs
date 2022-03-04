using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Essentials.Logging
{
    public class DebugLoggerOptions
    {
        public DebugLoggerOptions()
        {
            OutputLayout = DefaultLayout;
            Writer = DefaultWriter;

            AddFormatter<LogLevel>(level =>
            {
                if (level == LogLevel.Information)
                    return "INFO";
                return level.ToString().ToUpper().PadRight(5).Left(5);
            });
        }

        private static string DefaultLayout(string message, LogEntryState state)
        {
            return $"{state.Timestamp:HH:mm:ss.fff} [{{logLevel}}] {{category}} - {message}";
        }

        private static void DefaultWriter(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public List<Func<LogEntryState, bool>> LogFilters { get; } = new List<Func<LogEntryState, bool>>();

        public Dictionary<Type, Func<object, LogEntryState, string>> TypeFormatters { get; } = new Dictionary<Type, Func<object, LogEntryState, string>>();

        public Func<string, LogEntryState, string> OutputLayout { get; set; }

        public Action<string> Writer { get; set; }


        public DebugLoggerOptions AddFormatter<T>(Func<T, string> formatter)
        {
            TypeFormatters.Add(typeof(T), (value, state) => value is T t ? formatter(t) : string.Empty);
            return this;
        }

        public DebugLoggerOptions AddFormatter<T>(Func<T, LogEntryState, string> formatter)
        {
            TypeFormatters.Add(typeof(T), (value, state) => value is T t ? formatter(t, state) : string.Empty);
            return this;
        }

        public DebugLoggerOptions AddFilter(Func<LogEntryState, bool> filter)
        {
            LogFilters.Add(filter);
            return this;
        }

        public bool FiltersAllow(LogEntryState state)
        {
            return LogFilters.IsNullOrEmpty()
                || LogFilters.Any(f => f(state));
        }

    }
}
