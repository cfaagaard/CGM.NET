using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class InitiateMultiPacketTransferResponse : IBinaryType, IBinaryDeserializationSetting
    {



        //[BinaryElement(0)]
        //public UInt32 SegmentSize { get; set; }

        //[BinaryElement(4)]
        //public UInt16 PacketSize { get; set; }

        //[BinaryElement(6)]
        //public UInt16 LastPacketSize { get; set; }

        //[BinaryElement(8)]
        //public UInt16 PacketsToFetch { get; set; }


        [BinaryElement(0)]
        public UInt32 SegmentSize { get; set; }

        [BinaryElement(4)]
        public UInt16 PacketSize { get; set; }

        [BinaryElement(6)]
        public UInt16 LastPacketSize { get; set; }

        [BinaryElement(8)]
        public UInt16 PacketsToFetch { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.PumpDataHistory.AddMultiHandler(this); 
        }
    }
}
