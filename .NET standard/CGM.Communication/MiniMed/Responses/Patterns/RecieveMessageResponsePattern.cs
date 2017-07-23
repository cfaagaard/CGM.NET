using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class RecieveMessageResponsePattern: ReportPattern
    {
        public RecieveMessageResponsePattern():base(new byte[] { 0x80 }, 23)
        {

        }
    }
}
