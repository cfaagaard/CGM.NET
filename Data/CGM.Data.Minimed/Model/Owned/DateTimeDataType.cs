using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model.Owned
{
    public class DateTimeDataType
    {
        
        public Int32 Rtc { get; set; }

        public Int32 Offset { get; set; }

        public DateTime Date { get; set; }
        public double DateTimeEpoch { get; set; }
    }
}
