using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Patterns
{
    public class PumpHistoryRequestPattern : PumpRequestPattern
    {
        public PumpHistoryRequestPattern():base()
        {
            //length of message.......
            this.Patterns.Add(new ReportPattern(new byte[] { 0x11 }, 49));
            //this.Patterns.Add(new ReportPattern(new byte[] { 0x07 }, 50));
        }
    }
}
