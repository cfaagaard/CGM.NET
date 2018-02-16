using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize.Log
{
    public static class SessionLoggerExtension
    {
        public static ILoggerFactory AddSessionLog(this ILoggerFactory factory,
        SerializerSession session,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new SessionLoggerProvider(filter, session));
            return factory;
        }
    }
}
