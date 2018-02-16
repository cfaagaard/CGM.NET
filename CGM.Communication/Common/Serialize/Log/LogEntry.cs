using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize.Log
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }

        public override string ToString()
        {
            return $"{Date.ToString()} - {Message}";
        }

    }
}
