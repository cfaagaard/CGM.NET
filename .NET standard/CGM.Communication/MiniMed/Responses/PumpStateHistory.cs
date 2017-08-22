using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using System.Linq;

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
            //var lis = new List<byte>();
            //lis.AddRange(bytes);

            //if (bytes.Length >= settings.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.Init.PacketSize)
            //{
            //    this.Message = lis.GetRange(2, settings.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.Init.PacketSize).ToArray();
            //}
            //else
            //{
            //    //must be the last one.
            //    this.Message = lis.GetRange(2, settings.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.Init.LastPacketSize).ToArray();
            //}

            //this.AllBytes = bytes;
            //this.AllBytesE = bytes.Reverse().ToArray();
            //this.BytesAsString = BitConverter.ToString(AllBytes);

            settings.PumpDataHistory.CurrentMultiPacketHandler.CurrentSegment.AddHistory(this);//.PumpStateHistory.Add(this);
        }

        public override string ToString()
        {
            return $"{this.PacketNumber.ToString()}";
        }
    }

}
