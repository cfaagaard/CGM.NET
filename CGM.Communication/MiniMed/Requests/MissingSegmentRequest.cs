using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace CGM.Communication.MiniMed.Requests
{
    [BinaryType(IsLittleEndian = false)]
    public class MissingSegmentRequest : IBinaryType, IBinaryDeserializationSetting, IBinarySerializationSetting
    {

        [BinaryElement(0, Length = 2)]
        public byte[] StartPacket { get; set; }

        [BinaryElement(2, Length = 2)]
        public byte[] PacketCount { get; set; }

        //[BinaryElement(0,Length =2)]
        //public UInt16 StartPacket { get; set; }

        //[BinaryElement(2, Length = 2)]
        //public UInt16 PacketCount { get; set; }

        public MissingSegmentRequest(UInt16 startPacket, UInt16 packetCount)
        {
            //this.StartPacket = BitConverter.GetBytes(startPacket);
            //this.PacketCount = BitConverter.GetBytes(packetCount);

            this.StartPacket = BitConverter.GetBytes(startPacket).Reverse().ToArray();
            this.PacketCount = BitConverter.GetBytes(packetCount).Reverse().ToArray();

            //this.StartPacket = startPacket;
            //this.PacketCount = packetCount;
        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
          
        }

        public void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
          
        }
    }
}
