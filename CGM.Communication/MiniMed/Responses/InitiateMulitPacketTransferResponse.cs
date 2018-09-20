using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class InitiateMultiPacketTransferResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public Int32 SegmentSize { get; set; }

        [BinaryElement(4)]
        public UInt16 PacketSize { get; set; }

        [BinaryElement(6)]
        public UInt16 LastPacketSize { get; set; }

        [BinaryElement(8,Length =2)]
        public UInt16 PacketsToFetch { get; set; }


        //[BinaryElement(10,Length =2)]
        //public UInt16 Uknown1 { get; set; }

        //[BinaryElement(12, Length = 2)]
        //public UInt16 Uknown2 { get; set; }

        //[BinaryElement(10,Length =4)]
        //public int Uknown3 { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            if (settings.PumpDataHistory.CurrentMultiPacketHandler!=null)
            {
                settings.PumpDataHistory.CurrentMultiPacketHandler.AddSegmentHandler(this);
            }
            
        }
    }
}
