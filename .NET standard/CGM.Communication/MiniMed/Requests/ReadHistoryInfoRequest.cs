using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.DataTypes;

namespace CGM.Communication.MiniMed.Requests
{
    public class ReadHistoryRequest : ReadHistoryInfoRequest, IBinarySerializationSetting
    {
        public ReadHistoryRequest() : base()
        {

        }

        public ReadHistoryRequest(DateTime fromDateTime, DateTime toDateTime, HistoryDataTypeEnum historyDataType) : base(fromDateTime, toDateTime, historyDataType)
        {
        }

        public override void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
            settings.PumpDataHistory.SetCurrentMulitPacket(this);
        }

        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
            settings.PumpDataHistory.SetCurrentMulitPacket(this);
        }
    }

    [BinaryType(IsLittleEndian = false)]
    public class ReadHistoryInfoRequest : IBinaryType, IBinaryDeserializationSetting, IBinarySerializationSetting
    {
        // PUMP_DATA: 2,SENSOR_DATA: 3,
        [BinaryElement(0)]
        public byte HistoryDataType { get; set; }

        [BinaryElement(1)]
        public byte Unknown { get; set; }

        [BinaryElement(2)]
        public byte[] FromRtc { get; set; }

        [BinaryElement(6)]
        public byte[] ToRtc { get; set; }

        [BinaryElement(10)]
        public byte[] Unknown3 { get; set; }

        public DateTime? FromDateTime { get; set; }

        public DateTime? ToDateTime { get; set; }


        //public DateTimeDataType From { get; set; }
        //public DateTimeDataType To { get; set; }

        public ReadHistoryInfoRequest()
        {

        }

        public ReadHistoryInfoRequest(DateTime fromDateTime, DateTime toDateTime,HistoryDataTypeEnum historyDataType)
        {
            this.FromDateTime = fromDateTime;
            this.ToDateTime = toDateTime;

            this.FromRtc = fromDateTime.GetRtcBytes(-1665586902).Reverse().ToArray();
            //this.FromRtc = new byte[] { 0xea, 0x4e, 0x13, 0x84 };
            
            //this.ToRtc = toDateTime.GetRtcBytes(-1665586902).Reverse().ToArray();
            //does this means "read to the last record on the pump"......?
            this.ToRtc = new byte[] { 0xff, 0xff, 0xff, 0xff };
            this.HistoryDataType = (byte)historyDataType; // PUMP_DATA: 2,SENSOR_DATA: 3,
            this.Unknown = 0x04;
            this.Unknown3 = new byte[] { 0x00, 0x00 };
            //this.Unknown2 = (new byte[] { 0xff, 0xff, 0xff, 0xff }).GetInt32(0);
        }

        public virtual void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //this.From=new DateTimeDataType(FromRtc, settings.PumpTime.OffSet)
            this.FromDateTime = DateTimeExtension.GetDateTime(FromRtc, settings.PumpTime.OffSet);
            this.ToDateTime = DateTimeExtension.GetDateTime(ToRtc, settings.PumpTime.OffSet);
        }



        public override string ToString()
        {
            return this.FromDateTime.ToString();
        }

        public virtual void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
           
        }
    }
}
