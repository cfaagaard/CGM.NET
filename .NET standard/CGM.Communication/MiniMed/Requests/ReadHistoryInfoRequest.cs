using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests
{
    public class ReadHistoryRequest : ReadHistoryInfoRequest
    {
        public ReadHistoryRequest() : base()
        {

        }

        public ReadHistoryRequest(DateTime fromDateTime) : base(fromDateTime)
        {

            this.HistoryDataType = 0x02; // PUMP_DATA: 2,SENSOR_DATA: 3,
          
        }
    }

    [BinaryType(IsLittleEndian = false)]
    public class ReadHistoryInfoRequest : IBinaryType, IBinarySerializationSetting, IBinaryDeserializationSetting
    {
        // PUMP_DATA: 2,SENSOR_DATA: 3,
        [BinaryElement(0)]
        public byte HistoryDataType { get; set; }

        [BinaryElement(1)]
        public byte Unknown { get; set; }

        [BinaryElement(2)]
        public byte[] FromRtc { get; set; }

        [BinaryElement(6)]
        public int Unknown2 { get; set; }

        //[BinaryElement(6)]
        //public byte[] FromOffset { get; set; }

        [BinaryElement(10)]
        public byte[] Unknown3 { get; set; }

        public DateTime? FromDateTime { get; set; }

        //public DateTime? TestDateTime
        //{
        //    get
        //    {
        //        return DateTimeExtension.GetDateTime(FromRtc, Unknown2);
        //    }
        //}

        public DateTime? FromReadDateTime
        {
            get
            {
                if (FromDateTime.HasValue)
                {
                    var date = this.FromDateTime.Value.AddDays(-1);
                    return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                }
                return null;
            }
        }

        public ReadHistoryInfoRequest()
        {

        }

        public ReadHistoryInfoRequest(DateTime fromDateTime)
        {
            this.FromDateTime = fromDateTime;
            this.HistoryDataType = 0x03; // PUMP_DATA: 2,SENSOR_DATA: 3,
            this.Unknown =  0x04;
            this.Unknown3 = new byte[] { 0x00, 0x00 };
            this.Unknown2 = (new byte[] { 0xff, 0xff, 0xff, 0xff }).GetInt32(0);
        }
        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.FromDateTime = DateTimeExtension.GetDateTime(FromRtc, settings.PumpTime.OffSet);
        }

        public void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
            this.FromRtc = FromReadDateTime.Value.GetRtcBytes(settings.PumpTime.OffSet.GetInt32(0));
        }

        public override string ToString()
        {
            return this.FromReadDateTime?.ToString();
        }
    }
}
