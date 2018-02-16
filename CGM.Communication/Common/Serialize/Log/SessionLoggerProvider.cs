using System;
using Microsoft.Extensions.Logging;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;
using System.Collections.Generic;
using System.Threading;

namespace CGM.Communication.Common.Serialize.Log
{
    public class SessionLoggerProvider : ILoggerProvider
    {
        private SerializerSession _session;
        private Func<string, LogLevel, bool> _filter;

        public SessionLoggerProvider(Func<string, LogLevel, bool> filter, SerializerSession session)
        {
            _filter = filter;
            _session = session;
        }
        public FrameworkLogger CreateLogger(string categoryName)
        {
            return new SessionLogger(categoryName, _filter, _session);
        }

        public void Dispose()
        {
           
        }
    }

}
