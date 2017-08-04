using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Log
{
    public static class CgmLoggerExtension
    {
        public static ILoggerFactory AddCgmLog(this ILoggerFactory factory,
        string path,
        Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new CgmLoggerProvider(filter, path));
            return factory;
        }
    }
}
