using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class SendMessageResponsePattern: ReportPattern
    {
        public SendMessageResponsePattern():base(new byte[] { 0x81 }, 23)
        {

        }
    }
}
