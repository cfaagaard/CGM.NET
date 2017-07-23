using CGM.Communication.Interfaces;
using System.Collections.Generic;
using CGM.Communication.Patterns;
using CGM.Communication.Common;


namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class EnqOREotkPattern : ReportMultiPatternOr
    {
        public EnqOREotkPattern()
        {
            this.Patterns.Add(new ReportPattern(new byte[] { 001, ASCII.ENQ }, 4));
            this.Patterns.Add(new ReportPattern(new byte[] { 001, ASCII.EOT }, 4));
        }
    }
}
