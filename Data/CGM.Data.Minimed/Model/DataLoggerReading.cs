using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class DataLoggerReading
    {
        public int DataLoggerReadingId { get; set; }
        public int DataLoggerId { get; set; }
        public int BayerStickId { get; set; }

        public DateTime ReadingDateTime { get; set; }

        public DateTime? NextReadingDateTime { get; set; }

        public DataLogger DataLogger { get; set; }

        public BayerStick BayerStick { get; set; }

        public ICollection<PumpEvent> PumpEvents { get; set; }

        public DataLoggerReading()
        {
            PumpEvents = new HashSet<PumpEvent>();
        }
    }
}
