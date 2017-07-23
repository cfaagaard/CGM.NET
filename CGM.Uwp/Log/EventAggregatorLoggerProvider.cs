using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CGM.Uwp.Log
{
    public class EventAggregatorLoggerProvider : ILoggerProvider
    {
        private Func<string, LogLevel, bool> _filter;

        public EventAggregatorLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new EventAggregatorLogger(categoryName, _filter);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
