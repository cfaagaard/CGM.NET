using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests
{
    public class RemoteBolus
    {

        //def __init__(self, session, bolusID, amount, execute ):
        //unknown1 = 0 # ??
        //unknown2 = 0 # Square Wave amount?
        //unknown3 = 0 # Square Wave length?
        //payload = struct.pack( '>BBHHBH', bolusID, execute, unknown1, amount* 10000, unknown2, unknown3 )
        [BinaryElement(0)]
        public byte BolusID { get; set; }

        [BinaryElement(1)]
        public byte Execute { get; set; }

        [BinaryElement(2)]
        public byte unknown1 { get; set; }

        [BinaryElement(3)]
        public double Amount { get; set; }

      

        [BinaryElement(4)]
        public byte unknown2 { get; set; }

        [BinaryElement(5)]
        public byte unknown3 { get; set; }

    }
}
