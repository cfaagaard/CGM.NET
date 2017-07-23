using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Standard
{
    public class NAKMessage:AstmStart
    {
        public NAKMessage():base(new byte[] { 0x15 })
        {

        }
    }
}
