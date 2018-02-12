
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CGM.Communication.Log
{
    public class CgmLogger : ILogger
    {
        private string _path;
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;

        public CgmLogger(string categoryName, Func<string, LogLevel, bool> filter, string path)
        {
            _filter = filter;
            _path = path;
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

            string filename = DateTime.Now.ToString("ddMMyyyy") + "_log.txt";
            string path = Path.Combine(_path , filename);


            using (var stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.WriteLineAsync(message);
                }
            }
        }
    }
}