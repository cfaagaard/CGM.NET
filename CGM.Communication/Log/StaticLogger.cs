using CGM.Communication.Common.Serialize.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Log
{

    public static class StaticLogs
    {
        public static List<LogEntry> Logs { get; set; } = new List<LogEntry>();
    }

    public class StaticLogger : ILogger
    {


        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;


        public StaticLogger(string categoryName, Func<string, LogLevel, bool> filter)
        {

            _filter = filter;
            _categoryName = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (exception != null)
            {
                message += exception.ToString();
            }
            LogEntry lo = new LogEntry() { Date = DateTime.Now, LogLevel = logLevel, Category = _categoryName, Message = message };

            StaticLogs.Logs.Add(lo);


        }
    }


    public static class StaticLoggerExtensions
    {
        public static ILoggerFactory AddStaticLogger(this ILoggerFactory factory,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new StaticLoggerProvider(filter));
            return factory;
        }
    }

    public class StaticLoggerProvider : ILoggerProvider
    {
        private Func<string, LogLevel, bool> _filter;

        public StaticLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new OutputLogger(categoryName, _filter);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
