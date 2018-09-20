using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Event : BaseEvent
    {

        [JsonIgnore]
        [BinaryElement(0)]
        public byte MinutesBetweenReadings { get; set; }

        [JsonIgnore]
        [BinaryElement(1)]
        public byte NumberOfReadings { get; set; }

        [JsonIgnore]
        [BinaryElement(2)]
        public UInt16 PredictedSg { get; set; }


        [JsonIgnore]
        [BinaryElement(4)]
        [BinaryElementList(CountProperty = nameof(NumberOfReadings), Type = typeof(SENSOR_GLUCOSE_READINGS_EXTENDED_Detail), ByteSize = 9)]
        public List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> Details { get; set; } = new List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>();

        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
        }

        public override string ToString()
        {
            string returnstr = "";
            if (Details.Count >= 1)
            {
                returnstr = $"#:{NumberOfReadings} - {Details[0].ToString()}"; ;
            }
            return returnstr;
        }
    }


    [Serializable]
    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Detail : BaseEvent
    {

        [BinaryElement(0, Length = 1)]
        public byte SgvRaw1 { get; set; }

        [BinaryElement(1, Length = 1)]
        public byte SgvRaw2 { get; set; }

        private int _amount;
        public int Amount
        {
            get
            {
                _amount = ((SgvRaw1 & 3) << 8) | SgvRaw2;
                return _amount;
            }
            set { _amount = value; }
        }

        //[BinaryElement(3, Length = 2)]
        //public Int16 VCNTR { get; set; }

        [BinaryElement(2, Length = 2)]
        public Int16 IsigRaw { get; set; }

        private double _isig;
        public double Isig
        {
            get
            {
                _isig = (double)this.IsigRaw / 100;
                return _isig;
            }
            set { _isig = value; }
        }
        //maybe CgmTrend
        [BinaryElement(4, Length = 1)]
        public byte Uknown2 { get; set; }

        //this could be SensorRateOfChangeRaw
        [BinaryElement(5, Length = 2)]
        public Int16 RateOfChangeRaw { get; set; }

        private double _rateOfChange;
        public double RateOfChange
        {
            get
            {
                _rateOfChange = (double)this.RateOfChangeRaw / 100;
                return _rateOfChange;
            }
            set { _rateOfChange = value; }
        }


        private SgvAlert? sgvAlert;

        public SgvAlert? Alert
        {
            get
            {
                if (this.Amount > 700)
                {
                    sgvAlert = (SgvAlert)this.Amount;
                }
                return sgvAlert;
            }
            set { sgvAlert = value; }
        }



        [BinaryElement(7, Length = 1)]
        public byte SensorStatus { get; set; }




        //private long _epoch;
        //public long Epoch { get { _epoch=((DateTimeOffset)this.EventDate.DateTime.Value).ToUnixTimeMilliseconds();
        //        return _epoch;
        //    }
        //    set { _epoch = value; }
        //}

        //public string Reference { get; set; }

        //is set from parent
        public UInt16 PredictedSg { get; set; }

        //[BinaryElement(9, Length = 1)]
        //public byte ReadingStatus { get; set; }


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

        public override string ToString()
        {
            return $"{this.EventDate.DateTime.Value.ToString()} - {Amount}/{Isig}/{RateOfChange}/{PredictedSg}/{RateOfChangeRaw}";
        }
    }
}
