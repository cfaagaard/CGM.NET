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

        public ReadHistoryRequest(DateTime fromDateTime, HistoryDataTypeEnum historyDataType,byte[] offset) : base(fromDateTime, historyDataType, offset)
        {
        }

        public ReadHistoryRequest(int fromRtc, HistoryDataTypeEnum historyDataType) : base(fromRtc, historyDataType)
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
        public byte HistoryDataTypeRaw { get; set; }

        public HistoryDataTypeEnum HistoryDataType { get { return (HistoryDataTypeEnum)HistoryDataTypeRaw; } }

        [BinaryElement(1)]
        public byte Unknown { get; set; }

        [BinaryElement(2)]
        public byte[] FromRtc { get; set; }

        [BinaryElement(6)]
        public byte[] ToRtc { get; set; }

        [BinaryElement(10)]
        public byte[] Unknown3 { get; set; }

        public DateTime? FromDateTime { get; set; }

        public ReadHistoryInfoRequest()
        {

        }

        public ReadHistoryInfoRequest(DateTime fromDateTime,HistoryDataTypeEnum historyDataType,byte[] offset):this(fromDateTime.GetRtcBytes(offset).GetInt32(0),historyDataType)
        {
            this.FromDateTime = fromDateTime;
            this.FromRtc = fromDateTime.GetRtcBytes(offset);
         
       
        }

        public ReadHistoryInfoRequest(int fromRtc, HistoryDataTypeEnum historyDataType)
        {
            this.FromRtc =BitConverter.GetBytes(fromRtc);
            this.HistoryDataTypeRaw = (byte)historyDataType; 
            this.Unknown = 0x04;
            this.Unknown3 = new byte[] { 0x00, 0x00 };
            this.ToRtc = new byte[] { 0xff, 0xff, 0xff, 0xff };
        }

        public virtual void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.FromDateTime = settings.PumpTime.GetDateTime(FromRtc);
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
