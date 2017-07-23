using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{

        [BinaryType(IsLittleEndian = false)]
        public class PumpStateHistoryReadInfoResponse : IBinaryType, IBinaryDeserializationSetting
        {
        [BinaryElement(0)]
        public byte[] Unknown { get; set; }

        [BinaryElement(5)]
        public int FromDateTimeRtc { get; set; }

        [BinaryElement(9)]
        public int FromDateTimeOffSet { get; set; }

        public DateTime? FromDateTime { get { return DateTimeExtension.GetDateTime(this.FromDateTimeRtc, this.FromDateTimeOffSet); } }


        [BinaryElement(13)]
        public int ToDateTimeRtc { get; set; }

        [BinaryElement(17)]
        public int ToDateTimeOffSet { get; set; }

        public DateTime? ToDateTime { get { return DateTimeExtension.GetDateTime(this.ToDateTimeRtc, this.ToDateTimeOffSet); } }

        [BinaryElement(21)]
        public byte[] UnknownCrc16 { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.PumpDataHistory.ReadInfoResponse = this;
        }

        public override string ToString()
        {
            if (this.FromDateTime.HasValue && this.ToDateTime.HasValue)
            {
                return $"{this.FromDateTime.Value.ToString()} - {this.ToDateTime.Value.ToString()}";
            }
            return "(No dates)";
        }
    }



}
