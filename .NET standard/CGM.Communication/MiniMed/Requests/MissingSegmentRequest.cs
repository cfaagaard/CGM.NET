using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests
{
    [BinaryType(IsLittleEndian = false)]
    public class MissingSegmentRequest : IBinaryType, IBinaryDeserializationSetting, IBinarySerializationSetting
    {
        [BinaryElement(0,Length =2)]
        public UInt16 StartPacket { get; set; }

        [BinaryElement(2, Length = 2)]
        public UInt16 PacketCount { get; set; }

        public MissingSegmentRequest(UInt16 startPacket, UInt16 packetCount)
        {
            //this.StartPacket = BitConverter.GetBytes(startPacket);
            //this.PacketCount = BitConverter.GetBytes(packetCount);

            this.StartPacket = startPacket;
            this.PacketCount = packetCount;
        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
          
        }

        public void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
          
        }
    }
}
