using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmPayloadType :byte
    {
        Normal=0x00,
        Channel=0x04,
        Encrypted=0x06
    }
}
