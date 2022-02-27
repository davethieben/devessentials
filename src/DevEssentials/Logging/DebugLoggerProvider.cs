using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Essentials;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Essentials.Logging
{
    public class DebugLoggerProvider : ILoggerProvider
    {
        private readonly IOptions<DebugLoggerOptions> _options;

        public DebugLoggerProvider(IOptions<DebugLoggerOptions> options) => _options = options;

        public ILogger CreateLogger(string categoryName) => new DebugLogger(categoryName, _options);

        public void Dispose() { }

        private class DebugLogger : ILogger
        {
            private readonly HashSet<LogEntryState> _states = new HashSet<LogEntryState>();
            private readonly IOptions<DebugLoggerOptions> _options;

            public DebugLogger(string name, IOptions<DebugLoggerOptions> options)
            {
                Name = name;
                _options = options;
            }

            public string Name { get; }

            public IDisposable BeginScope<TState>(TState state)
            {
                var debugState = new LogEntryState(state);
                if (debugState.Any())
                {
                    _states.Add(debugState);
                    return new Disposable(() =>
                    {
                        _states.Remove(debugState);
                    });
                }
                else
                {
                    return Disposable.NullDisposable;
                }
            }

            public bool IsEnabled(LogLevel logLevel) => Debugger.IsAttached && logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> paramFormatter)
            {
                if (!IsEnabled(logLevel))
                    return;

                DateTime timestamp = DateTime.Now;
                string message = paramFormatter.IsRequired().Invoke(state, exception);

                if (!string.IsNullOrEmpty(message))
                {
                    using (BeginScope(state))
                    using (BeginScope(new { category = Name, logLevel, eventId, exception, timestamp }))
                    {
                        var options = _options.Value ?? new DebugLoggerOptions();
                        var combinedState = LogEntryState.Combine(_states);

                        if (options.FiltersAllow(combinedState))
                        {
                            message = options.OutputLayout(message, combinedState);

                            if (HasTokens(message))
                            {
                                var stringState = combinedState.ToDictionary(
                                    keySelector: pair => pair.Key,
                                    elementSelector: pair =>
                                    {
                                        if (pair.Value != null && options.TypeFormatters.TryGetValue(pair.Value.GetType(), out var formatter))
                                        {
                                            return formatter.Invoke(pair.Value, combinedState);
                                        }
                                        else
                                        {
                                            return (pair.Value?.ToString()) ?? "";
                                        }
                                    });

                                message = new StringReplacement(message, stringState);
                            }

                            options.Writer(message);
                        }
                    }
                }
            }

            private static bool HasTokens(string input) =>
                !string.IsNullOrEmpty(input) && input.Contains(StringReplacement.TokenStart) && input.Contains(StringReplacement.TokenEnd);
        }
    }
}
