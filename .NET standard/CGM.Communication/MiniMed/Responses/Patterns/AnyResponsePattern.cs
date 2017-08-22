using CGM.Communication.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Patterns
{
    public class AnyResponsePattern : ReportPattern
    {

        public AnyResponsePattern() : base(new byte[] { 0 }, 0)
        {

        }
    }
}
