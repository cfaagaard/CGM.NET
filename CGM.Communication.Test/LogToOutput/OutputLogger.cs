using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Test.LogToOutput
{
    public class OutputLogger : ILogger
    {


        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
   

        public OutputLogger(string categoryName, Func<string, LogLevel, bool> filter)
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

            //message = $"{DateTime.Now} - { logLevel }: {_categoryName} : {message}";
            //message += Environment.NewLine;
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }
            LogEntry lo = new LogEntry() { Date = DateTime.Now, LogLevel = logLevel, Category = _categoryName, Message = message };

            System.Diagnostics.Debug.WriteLine($"{lo.Date} - {lo.Message}");


        }
    }

    public class LogEntry
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }

        public override string ToString()
        {
            return $"{Date.ToString()} - {Message}";
        }

    }

    public static class OutputLoggerExtensions
    {
        public static ILoggerFactory AddOutputLogger(this ILoggerFactory factory,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new OutputLoggerProvider(filter));
            return factory;
        }
    }

    public class OutputLoggerProvider : ILoggerProvider
    {
        private Func<string, LogLevel, bool> _filter;

        public OutputLoggerProvider(Func<string, LogLevel, bool> filter)
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
