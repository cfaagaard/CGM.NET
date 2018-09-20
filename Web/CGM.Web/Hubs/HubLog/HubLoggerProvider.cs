using System;
using Microsoft.Extensions.Logging;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;
using System.Collections.Generic;
using System.Threading;

namespace CGM.Web.Hubs.HubLog
{
    public class HubLoggerProvider : ILoggerProvider
    {

        private Func<string, LogLevel, bool> _filter;
        private readonly DataLoggerHub hub;

        public HubLoggerProvider(Func<string, LogLevel, bool> filter, DataLoggerHub hub)
        {
            _filter = filter;
            this.hub = hub;
        }
        public FrameworkLogger CreateLogger(string categoryName)
        {
            return new HubLogger(categoryName, _filter,hub);
        }

        public void Dispose()
        {
           
        }
    }

}
