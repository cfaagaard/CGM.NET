
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class BasePumpEvent : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte EventTypeRaw { get; set; }

        private EventTypeEnum _eventType;

       
        public EventTypeEnum EventType { get { _eventType=(EventTypeEnum)EventTypeRaw;
                return _eventType;
            }
            set { _eventType = value; }
        }
        [BinaryElement(1)]
        public byte Source { get; set; }

        [BinaryElement(2)]
        public byte Length { get; set; }

        [BinaryElement(3)]
        public DateTimeDataType EventDate { get; set; }

        //[BinaryElement(3)]
        //public int Rtc { get; set; }
        //[BinaryElement(7)]
        //public int Offset { get; set; }
        //public DateTime? Timestamp { get { return DateTimeExtension.GetDateTime(this.Rtc, this.Offset); } }



       
        [JsonIgnore]
        [BinaryElement(11)]
        [MessageType(typeof(SENSOR_GLUCOSE_READINGS_EXTENDED_Event), nameof(EventType), EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED)]
        [MessageType(typeof(BOLUS_WIZARD_ESTIMATE_Event), nameof(EventType), EventTypeEnum.BOLUS_WIZARD_ESTIMATE)]
        [MessageType(typeof(CANNULA_FILL_DELIVERED_Event), nameof(EventType), EventTypeEnum.CANNULA_FILL_DELIVERED)]
        [MessageType(typeof(ALARM_NOTIFICATION_Event), nameof(EventType), EventTypeEnum.ALARM_NOTIFICATION)]
        [MessageType(typeof(ALARM_CLEARED_Event), nameof(EventType), EventTypeEnum.ALARM_CLEARED)]
        [MessageType(typeof(BG_READING_Event), nameof(EventType), EventTypeEnum.BG_READING)]
        [MessageType(typeof(PLGM_CONTROLLER_STATE_Event), nameof(EventType), EventTypeEnum.PLGM_CONTROLLER_STATE)]
        [MessageType(typeof(TEMP_BASAL_PROGRAMMED_Event), nameof(EventType), EventTypeEnum.TEMP_BASAL_PROGRAMMED)]
        [MessageType(typeof(EXERCISE_MARKER_Event), nameof(EventType), EventTypeEnum.EXERCISE_MARKER)]
        [MessageType(typeof(INJECTION_MARKER_Event), nameof(EventType), EventTypeEnum.INJECTION_MARKER)]
        [MessageType(typeof(FOOD_MARKER_Event), nameof(EventType), EventTypeEnum.FOOD_MARKER)]
        [MessageType(typeof(OTHER_MARKER_Event), nameof(EventType), EventTypeEnum.OTHER_MARKER)]
        [MessageType(typeof(DAILY_TOTALS_Event), nameof(EventType), EventTypeEnum.DAILY_TOTALS)]
        [MessageType(typeof(LOW_RESERVOIR_Event), nameof(EventType), EventTypeEnum.LOW_RESERVOIR)]
        [MessageType(typeof(BOLUS_Event), nameof(EventType), EventTypeEnum.NORMAL_BOLUS_PROGRAMMED)]
        [MessageType(typeof(BOLUS_Event), nameof(EventType), EventTypeEnum.SQUARE_BOLUS_PROGRAMMED)]
        [MessageType(typeof(BOLUS_Event), nameof(EventType), EventTypeEnum.DUAL_BOLUS_PROGRAMMED)]
       
        [MessageType(typeof(NORMAL_BOLUS_DELIVERED_Event), nameof(EventType), EventTypeEnum.NORMAL_BOLUS_DELIVERED)]
        [MessageType(typeof(SQUARE_BOLUS_DELIVERED_Event), nameof(EventType), EventTypeEnum.SQUARE_BOLUS_DELIVERED)]
        [MessageType(typeof(DUAL_BOLUS_DELIVERED_Event), nameof(EventType), EventTypeEnum.DUAL_BOLUS_PART_DELIVERED)]
        [MessageType(typeof(CALIBRATION_COMPLETE_Event), nameof(EventType), EventTypeEnum.CALIBRATION_COMPLETE)]
        [MessageType(typeof(INSULIN_DELIVERY_RESTARTED_Event), nameof(EventType), EventTypeEnum.INSULIN_DELIVERY_RESTARTED)]
        [MessageType(typeof(INSULIN_DELIVERY_STOPPED_Event), nameof(EventType), EventTypeEnum.INSULIN_DELIVERY_STOPPED)]

        

        [MessageType(typeof(BaseEvent))]
        [BinaryPropertyValueTransfer(ChildPropertyName = nameof(EventDate), ParentPropertyName = nameof(EventDate))]
        public BaseEvent Message { get; set; }


       
        [JsonIgnore]
        public byte[] AllBytes { get; set; }

       
        [JsonIgnore]
        public byte[] AllBytesE { get; set; }
     
        public string BytesAsString { get; set; }
        public int Index { get; set; }

        private string _key;
        public string Key
        {
            get
            {

                if (string.IsNullOrEmpty(_key))
                {
                    //string bytes = BytesAsString.Remove(20, 12);
                    //_key = string.Format("{0}_{1}", HistoryDataType, bytes);
                    _key = $"{HistoryDataType}{(byte)EventType}{EventDate.Rtc}";
                }
                return _key;
            }
        }

        public int HistoryDataType { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
            this.BytesAsString = BitConverter.ToString(AllBytes);
            if (this.Message!=null && this.Message.GetType().Equals(typeof(SENSOR_GLUCOSE_READINGS_EXTENDED_Event)))
            {
                try
                {
                    var reading = (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)this.Message;
                    for (int i = 0; i < reading.Details.Count; i++)
                    {
                        if (reading.EventDate.DateTime.HasValue)
                        {
                            var read = reading.Details[i];
                            var readingRtc = this.EventDate.Rtc - (i * reading.MinutesBetweenReadings * 60);
                            read.EventDate =new DateTimeDataType(readingRtc, this.EventDate.Offset); //DateTimeExtension.GetDateTime(readingRtc, this.EventDate.Offset);
                            read.PredictedSg = reading.PredictedSg;
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
    
            }
        }

        public override string ToString()
        {
            if (this.Message != null && !string.IsNullOrEmpty(this.Message.ToString()))
            {
                return $"{EventDate.DateTime.Value} - {EventType.ToString()} - {this.Message.ToString()}";
            }
            else
            {
                return $"{EventDate.DateTime.Value} - {EventType.ToString()}";
            }

        }

    }

    [BinaryType(IsLittleEndian = false)]
    public class BaseEvent : IBinaryType, IBinaryDeserializationSetting
    {
       
        public DateTimeDataType EventDate { get; set; }

       
        [JsonIgnore]
        public byte[] AllBytes { get; set; }
      
        [JsonIgnore]
        public byte[] AllBytesE { get; set; }
        public string BytesAsString { get; set; }
        public virtual void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
            this.BytesAsString = BitConverter.ToString(AllBytes);
        }

        public override string ToString()
        {
            return "";
        }
    }

}
