using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Patterns
{
    public class EHSMRequestPattern : PumpRequestPattern
    {
        public EHSMRequestPattern():base()
        {
            //length of message.......
            this.Patterns.Add(new ReportPattern(new byte[] { 0x06 }, 50));
        }
    }
}
