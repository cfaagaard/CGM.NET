using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Uwp.Log
{
    public static class EventAggregatorLoggerExtensions
    {
        public static ILoggerFactory AddEventAggregatorLog(this ILoggerFactory factory,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new EventAggregatorLoggerProvider(filter));
            return factory;
        }
    }
}
