using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Patterns
{


    public class LinkKeyResponsePattern : ReportMultiPatternAnd
    {
        public LinkKeyResponsePattern()
        {
            this.Patterns.Add(new ReportPattern(new byte[] { 0x41 }, 1));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x51 }, 5));
            //this.Patterns.Add(new ReportPattern(new byte[] { 0x14 }, 23));
            this.Patterns.Add(new ReportPattern(new byte[] { 0x86 }, 23));
        }
    }
}
