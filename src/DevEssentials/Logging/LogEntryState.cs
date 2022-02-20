using System;
using System.Collections.Generic;
using Essentials.Reflection;
using Microsoft.Extensions.Logging;

namespace Essentials.Logging
{
    public class LogEntryState : Dictionary<string, object?>
    {
        public LogEntryState(object? state = null)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (state != null)
            {
                if (state is IEnumerable<KeyValuePair<string, object?>> pairs)
                    this.AddRange(pairs);
                else
                    state.CopyToDictionary(this);
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (var pair in this)
                hashCode ^= pair.Key.GetHashCode() ^ (pair.Value?.GetHashCode() ?? 0);
            return hashCode;
        }

        public LogLevel LogLevel => TryGetValue("logLevel", out object? value) && value is LogLevel logLevel ? logLevel : LogLevel.None;

        public string? Category => TryGetValue("category", out object? value) && value is string category ? category?.ToString() : null;

        public DateTime Timestamp => TryGetValue("timestamp", out object? value) && value is DateTime timestamp ? timestamp : DateTime.Now;

        public static LogEntryState Combine(IEnumerable<LogEntryState> states)
        {
            var output = new LogEntryState();
            foreach (var dictionary in states.EmptyIfNull())
            {
                output.AddRange(dictionary);
            }
            return output;
        }

    }
}
