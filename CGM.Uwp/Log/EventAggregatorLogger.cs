using CGM.Communication.Common.Serialize.Log;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Uwp.Log
{
    public class EventAggregatorLogger : ILogger
    {


        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
   

        public EventAggregatorLogger(string categoryName, Func<string, LogLevel, bool> filter)
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

            Messenger.Default.Send<LogEntry>(lo);


        }
    }
}
