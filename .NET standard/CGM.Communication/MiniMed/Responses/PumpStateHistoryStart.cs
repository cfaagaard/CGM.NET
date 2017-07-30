using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGM.Communication.Extensions;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class PumpStateHistoryStart : IBinaryType, IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public Int16 Start { get; set; }

        [BinaryElement(2)]
        public byte DataType { get; set; }

        [BinaryElement(3)]
        public Int32 historySizeCompressed { get; set; }

        [BinaryElement(7)]
        public Int32 historySizeUncompressed { get; set; }

        [BinaryElement(11)]
        public byte historyCompressed { get; set; }

        [BinaryElement(12)]
        public byte[] Uknown1 { get; set; }

        [BinaryElement(17)]
        public int DateTimeRtc { get; set; }

        [BinaryElement(21)]
        public int DateTimeOffSet { get; set; }

        public DateTime? DateTime { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc, this.DateTimeOffSet); } }

        [BinaryElement(25)]
        public byte[] Uknown2 { get; set; }

        [BinaryElement(41)]
        public int DateTimeRtc2 { get; set; }

        public DateTime? DateTime2 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc2, this.DateTimeOffSet); } }

        [BinaryElement(45)]
        public byte[] Uknown3 { get; set; }

        [BinaryElement(92,Length =4)]
        public int DateTimeRtc3 { get; set; }

        public DateTime? DateTime3 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc3, this.DateTimeOffSet); } }

        [BinaryElement(113, Length = 4)]
        public int DateTimeRtc4 { get; set; }

        public DateTime? DateTime4 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc4, this.DateTimeOffSet); } }

        [BinaryElement(157, Length = 4)]
        public int DateTimeRtc5 { get; set; }

        public DateTime? DateTime5 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc5, this.DateTimeOffSet); } }


        [BinaryElement(203, Length = 4)]
        public int DateTimeRtc6 { get; set; }

        public DateTime? DateTime6 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc6, this.DateTimeOffSet); } }

        [BinaryElement(266, Length = 4)]
        public int DateTimeRtc7 { get; set; }

        public DateTime? DateTime7 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc7, this.DateTimeOffSet); } }

        [BinaryElement(350, Length = 4)]
        public int DateTimeRtc8 { get; set; }

        public DateTime? DateTime8 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc8, this.DateTimeOffSet); } }


        [BinaryElement(2290, Length = 4)]
        public int DateTimeRtc9 { get; set; }

        public DateTime? DateTime9 { get { return DateTimeExtension.GetDateTime(this.DateTimeRtc9, this.DateTimeOffSet); } }
        [BinaryElement(163)]
        public byte[] Payload { get; set; }


        public byte[] AllBytes { get; set; }

        public byte[] AllBytesE { get; set; }


        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //throw new NotImplementedException();
            List<byte> list = bytes.ToList();//.GetRange(11,bytes.Length-11);

            this.AllBytes = list.ToArray() ;
            this.AllBytesE = list.ToArray().Reverse().ToArray();
        }
    }
}
