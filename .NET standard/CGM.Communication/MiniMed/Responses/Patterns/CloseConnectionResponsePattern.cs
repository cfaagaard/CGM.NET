using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class CloseConnectionResponsePattern : ReportMultiPatternAnd
    {
        public CloseConnectionResponsePattern() 
        {
            this.Patterns.Add(new ReportPattern(new byte[] { 0x41 }, 1));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x51 }, 5));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x11 }, 23));
            //this.Patterns.Add(new ReportPattern(new byte[] { 0x55 }, 38));
        }
    }
}
