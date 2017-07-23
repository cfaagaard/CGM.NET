using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common
{
    public class ByteFrame
    {
        public int Framenumber { get; set; }
        public byte[] Bytes { get; set; }

        public bool MoreBytes { get { return Bytes[4] == 0x3C; } }

        public string  Comment { get; set; }

    }
}
