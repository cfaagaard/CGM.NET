using CGM.Communication.Common;
using CGM.Communication.Patterns;
using System.Collections.Generic;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class MedtronicMessagePattern : ReportPattern
    {
        public MedtronicMessagePattern() : base(new byte[] { 0x51, 0x03 }, 5)
        {

        }

    }

}
