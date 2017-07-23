using CGM.Communication.Common;
using CGM.Communication.Patterns;
using System.Collections.Generic;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class ACKPattern : ReportPattern
    {
        public ACKPattern() : base(new byte[] { 001, ASCII.ACK }, 4)
        {

        }

    }

}
