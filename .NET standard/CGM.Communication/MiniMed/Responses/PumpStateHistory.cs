using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using System.Linq;
using CGM.Communication.Common;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian=false)]
    public class PumpStateHistory : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0, Length = 2)]
        public Int16 PacketNumber { get; set; }

        [BinaryElement(2)]
        public byte[] FullMessage { get; set; }

        //public byte[] Message { get; set; }


        //public byte[] AllBytes { get; set; }
        //public byte[] AllBytesE { get; set; }

        //public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            var lis = new List<byte>();
            lis.AddRange(bytes);
            int length = lis.Count;



            //var blockChecksum1 = lis.ToArray().GetUInt16BigE(length - 4);
            //var blockChecksum2 = lis.ToArray().GetUInt16BigE(length - 2);
            //var blockChecksum3 = lis.ToArray().GetUInt16BigE(length - 6);

            //var blockChecksum4 = lis.ToArray().GetUInt16(length - 4);
            //var blockChecksum5 = lis.ToArray().GetUInt16(length - 2);
            //var blockChecksum6 = lis.ToArray().GetUInt16(length - 6);

            //var byteToCheck1 = lis.GetRange(2, length - 4);
            //var byteToCheck2 = lis.GetRange(2, length - 6);
            //var byteToCheck3 = lis.GetRange(2, length - 2);
            //var byteToCheck4 = lis.GetRange(0, length - 2);
            //var byteToCheck5 = lis.GetRange(0, length - 4);
            //var byteToCheck6 = lis.GetRange(0, length - 6);

            //var calculatedChecksum1 = Crc16Ccitt.CRC16CCITT(byteToCheck1.ToArray(), 0xFFFF, 0x1021, byteToCheck1.Count);
            //var calculatedChecksum2 = Crc16Ccitt.CRC16CCITT(byteToCheck2.ToArray(), 0xFFFF, 0x1021, byteToCheck2.Count);
            //var calculatedChecksum3 = Crc16Ccitt.CRC16CCITT(byteToCheck3.ToArray(), 0xFFFF, 0x1021, byteToCheck3.Count);

            //var calculatedChecksum4 = Crc16Ccitt.CRC16CCITT(byteToCheck4.ToArray(), 0xFFFF, 0x1021, byteToCheck4.Count);
            //var calculatedChecksum5 = Crc16Ccitt.CRC16CCITT(byteToCheck5.ToArray(), 0xFFFF, 0x1021, byteToCheck5.Count);
            //var calculatedChecksum6 = Crc16Ccitt.CRC16CCITT(byteToCheck6.ToArray(), 0xFFFF, 0x1021, byteToCheck6.Count);

            settings.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.AddHistory(this);
        }

        public override string ToString()
        {
            return $"{this.PacketNumber.ToString()}";
        }
    }

}
