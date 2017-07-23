using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Standard
{
    public class EOTMessage : AstmStart
    {
        public EOTMessage() : base(new byte[] { 0x04 })
        {

        }
    }
}
