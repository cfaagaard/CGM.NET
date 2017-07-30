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
        public byte[] SeqNumber { get; set; }

        //[BinaryElement(2)]
        //public byte[] Start { get; set; }

        //[BinaryElement(12)]
        //public byte[] Unknown0 { get; set; }

        //[BinaryElement(19)]
        //public byte[] ToDateTimeRtc { get; set; }

        //[BinaryElement(23)]
        //public byte[] ToDateTimeOffSet { get; set; }

        //public DateTime? ToDateTime { get { return DateTimeExtension.GetDateTime(this.ToDateTimeRtc, this.ToDateTimeOffSet); } }

        [BinaryElement(2)]
        public byte[] Message { get; set; }

        //[BinaryElement(1000,Length =2)]
        //public byte[] ccr { get; set; }
        //[BinaryElement(99)]
        //public byte[] Unknown2 { get; set; }

        public byte[] AllBytes { get; set; }
        public byte[] AllBytesE { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //var list = new List<byte>();
            //list.AddRange(bytes);
            //this.AllBytes = list.GetRange(3,list.Count-3).ToArray();
            var lis = new List<byte>();
            lis.AddRange(bytes);
            //maybe there is two times ccr at the end 
            this.Message = lis.GetRange(2, bytes.Length - 6).ToArray();
            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
            this.BytesAsString = BitConverter.ToString(AllBytes);

            settings.PumpDataHistory.CurrentMultiPacketHandler.PumpStateHistory.Add(this);
        }

        public override string ToString()
        {
            return $"{BitConverter.ToString(this.SeqNumber)}";
        }
    }

}
