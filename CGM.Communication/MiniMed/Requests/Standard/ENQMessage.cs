using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests.Standard
{
    public class ENQMessage : AstmStart
    {
        public ENQMessage() : base(new byte[] { 0x05 })
        {

        }

    }

}
