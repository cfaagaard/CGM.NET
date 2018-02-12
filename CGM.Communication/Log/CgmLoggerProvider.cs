using System;
using Microsoft.Extensions.Logging;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;
using System.Collections.Generic;
using System.Threading;

namespace CGM.Communication.Log
{
    public class CgmLoggerProvider : ILoggerProvider
    {
        private string _path;
        private Func<string, LogLevel, bool> _filter;

        public CgmLoggerProvider(Func<string, LogLevel, bool> filter, string path)
        {
            _filter = filter;
            _path = path;
        }
        public FrameworkLogger CreateLogger(string categoryName)
        {
            return new CgmLogger(categoryName, _filter, _path);
        }

        public void Dispose()
        {
           
        }
    }

}
