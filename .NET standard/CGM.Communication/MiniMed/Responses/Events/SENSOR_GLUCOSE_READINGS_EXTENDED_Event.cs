using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Event:BaseEvent
    {
        [BinaryElement(0)]
        public byte MinutesBetweenReadings { get; set; }

        [BinaryElement(1)]
        public byte NumberOfReadings { get; set; }

        [BinaryElement(2)]
        public byte PredictedSg { get; set; }

        [BinaryElement(3)]
        [BinaryElementList(CountProperty = nameof(NumberOfReadings), Type = typeof(SENSOR_GLUCOSE_READINGS_EXTENDED_Detail), ByteSize = 9)]
        public List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> Details { get; set; } = new List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>();

        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
        }
    }


  
    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Detail : BaseEvent
    {
        [BinaryElement(0,Length =1)]
        public byte PREDICTED_SENSOR_GLUCOSE_AMOUNT { get; set; }

        [BinaryElement(1, Length = 1)]
        public byte Uknown1 { get; set; }

        [BinaryElement(2, Length = 1)]
        public byte AMOUNT { get; set; }

        //[BinaryElement(3, Length = 2)]
        //public Int16 VCNTR { get; set; }

        [BinaryElement(3,Length =2)]
        public Int16 IsigRaw { get; set; }

        public double Isig { get { return (double)this.IsigRaw / 100; } }

        [BinaryElement(5, Length = 1)]
        public byte Uknown2 { get; set; }

        [BinaryElement(6)]
        public Int16 RateOfChangeRaw { get; set; }

        public double RateOfChange { get { return (double)this.RateOfChangeRaw / 100; } }

        [BinaryElement(8)]
        public byte SensorStatus { get; set; }

        [BinaryElement(9)]
        public byte ReadingStatus { get; set; }

        //TODO

        //const backfilledData = (readingStatus & 1) === 1;
        //const settingsChanged = (readingStatus & 2) === 1;
        //const noisyData = sensorStatus === 1;
        //const discardData = sensorStatus === 2;
        //const sensorError = sensorStatus === 3;

    //    const vctr = NGPUtil.make32BitIntFromNBitSignedInt(

    //(((this.eventData[pos] >> 2) & 3) << 8) | this.eventData[pos + 4], 10) / 100.0;

        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
        }
    }
}
