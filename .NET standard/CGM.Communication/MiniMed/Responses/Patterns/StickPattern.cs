using CGM.Communication.Common;
using CGM.Communication.Patterns;
using System.Collections.Generic;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class StickPattern : ReportPattern
    {
        public StickPattern() : base(new byte[] { 0x04, 0x02 }, 5)
        {

        }

    }

}
