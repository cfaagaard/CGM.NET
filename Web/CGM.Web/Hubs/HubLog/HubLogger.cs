
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CGM.Web.Hubs.HubLog
{
    public class HubLogger : ILogger
    {
 
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private readonly DataLoggerHub hub;

        public HubLogger(string categoryName, Func<string, LogLevel, bool> filter, DataLoggerHub hub)
        {
            _filter = filter;
            this.hub = hub;
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

           // message = $"{DateTime.Now} - { logLevel }: {_categoryName} : {message}";
            message = $"{DateTime.Now}; {message}";
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            if (logLevel==LogLevel.Information)
            {
                hub.SendMessage(message);
            }
            

        }
    }
}