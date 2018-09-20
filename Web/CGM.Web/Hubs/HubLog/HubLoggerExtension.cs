using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Web.Hubs.HubLog
{
    public static class HubLoggerExtension
    {
        public static ILoggerFactory AddHubLogger(this ILoggerFactory factory,
            DataLoggerHub hub,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new HubLoggerProvider(filter,hub));
            return factory;
        }
    }
}
