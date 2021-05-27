using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Essentials.Logging
{
    public class StreamLogger : ILogger, IDisposable
    {
        private readonly string? _name;
        private readonly StreamWriter _writer;
        private readonly Stack<StreamLogger> _loggers = new Stack<StreamLogger>();

        public StreamLogger(string? name, Stream stream)
        {
            _name = name;
            _writer = new StreamWriter(stream);
            _loggers.Push(this);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            _writer.Flush();
            _loggers.Push(new StreamLogger(state?.ToString(), _writer.BaseStream));
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_loggers.Count > 1)
                _loggers.Peek().Log(logLevel, eventId, state, exception, formatter);
            else
                _writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")} [{logLevel}] {_name} - {formatter(state, exception)}");
        }

        public void Dispose()
        {
            if (_loggers.Count > 1)
                _loggers.Pop().Dispose();
            else
                _writer.Dispose();
        }

    }
}
