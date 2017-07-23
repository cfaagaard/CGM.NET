using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Patterns
{
    public class OpenConnectionRequestPattern : ReportMultiPatternAnd
    {
        public OpenConnectionRequestPattern() 
        {
            this.Patterns.Add(new ReportPattern(new byte[] { 0x00 }, 2));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x51 }, 5));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x10 }, 23));
 
        }
    }
}
