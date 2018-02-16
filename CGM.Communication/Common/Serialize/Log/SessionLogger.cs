
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CGM.Communication.Common.Serialize.Log
{
    public class SessionLogger : ILogger
    {
        private SerializerSession _session;
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;

        public SessionLogger(string categoryName, Func<string, LogLevel, bool> filter, SerializerSession session)
        {
            _filter = filter;
            _session = session;
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

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
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
                message +=  exception.ToString();
            }
            LogEntry lo = new LogEntry() { Date = DateTime.Now, LogLevel = logLevel, Category = _categoryName, Message = message };
           // _session.Logs.Add(lo);

         
        }
    }
}