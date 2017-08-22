using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [BinaryType(IsLittleEndian = false)]
    public class PumpEvent : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte EventTypeRaw { get; set; }
        public EventTypeEnum EventType { get { return (EventTypeEnum)EventTypeRaw; } }
        [BinaryElement(1)]
        public byte Source { get; set; }

        [BinaryElement(2)]
        public byte Length { get; set; }
        [BinaryElement(3)]
        public int Rtc { get; set; }
        [BinaryElement(7)]
        public int Offset { get; set; }
        public DateTime? Timestamp { get { return DateTimeExtension.GetDateTime(this.Rtc, this.Offset); } }

        [BinaryElement(11)]
        [MessageType(typeof(SENSOR_GLUCOSE_READINGS_EXTENDED_Event), nameof(EventType), EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED)]
        [MessageType(typeof(BOLUS_WIZARD_ESTIMATE_Event), nameof(EventType), EventTypeEnum.BOLUS_WIZARD_ESTIMATE)]
        [MessageType(typeof(CANNULA_FILL_DELIVERED_Event), nameof(EventType), EventTypeEnum.CANNULA_FILL_DELIVERED)]
        [MessageType(typeof(ALARM_NOTIFICATION_Event), nameof(EventType), EventTypeEnum.ALARM_NOTIFICATION)]
        [MessageType(typeof(BG_READING_Event), nameof(EventType), EventTypeEnum.BG_READING)]
        [MessageType(typeof(BaseEvent))]
        [BinaryPropertyValueTransfer(ChildPropertyName = nameof(Timestamp),ParentPropertyName = nameof(Timestamp))]
        public BaseEvent Message { get; set; }
        
        public byte[] AllBytes { get; set; }
        public byte[] AllBytesE { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
        }

        public override string ToString()
        {
            if (this.Message!=null)
            {
                return $"{Timestamp.Value} - {EventType.ToString()} - {this.Message.ToString()}";
            }
            else
            {
                return $"{Timestamp.Value} - {EventType.ToString()}";
            }
        
        }

    }

    [BinaryType(IsLittleEndian = false)]
    public class BaseEvent : IBinaryType, IBinaryDeserializationSetting
    {

        public DateTime? Timestamp { get; set; }

        public byte[] AllBytes { get; set; }
        public byte[] AllBytesE { get; set; }
        public string BytesAsString { get; set; }
        public virtual void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
            this.BytesAsString = BitConverter.ToString(AllBytes);
        }
    }

}
