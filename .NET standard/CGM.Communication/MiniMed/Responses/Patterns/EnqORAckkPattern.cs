using System.Collections.Generic;
using CGM.Communication.Patterns;
using CGM.Communication.Common;


namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class EnqORAckkPattern : ReportMultiPatternOr
    {
        public EnqORAckkPattern() 
        {
            this.Patterns.Add(new ReportPattern(new byte[] { 001, ASCII.ENQ }, 4));
            this.Patterns.Add(new ReportPattern(new byte[] { 001, ASCII.ACK }, 4));
        }
    }

}
