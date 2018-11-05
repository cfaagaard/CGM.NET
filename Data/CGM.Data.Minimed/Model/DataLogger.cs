using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class DataLogger
    {
        public int DataLoggerId { get; set; }
        public string DataLoggerName { get; set; }
        public string DataLoggerKey { get; set; }

        public ICollection<DataLoggerReading> DataLoggerReadings { get; set; }

        public DataLogger()
        {
            DataLoggerReadings = new HashSet<DataLoggerReading>();
        }
    }
}
