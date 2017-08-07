using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Event : BaseEvent
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
            for (int i = 0; i < Details.Count; i++)
            {
                if (this.Timestamp.HasValue)
                {
                    Details[i].Timestamp = this.Timestamp.Value.AddMinutes((i * this.MinutesBetweenReadings));
                }

            }

        }

        public override string ToString()
        {
            string returnstr = "";
            if (Details.Count >= 1)
            {
                returnstr = $"{this.Timestamp.Value.ToString()} - {Details[0].ToString()}"; ;
            }
            return returnstr;
        }
    }



    public class SENSOR_GLUCOSE_READINGS_EXTENDED_Detail : BaseEvent
    {
        [BinaryElement(0, Length = 1)]
        public byte PREDICTED_SENSOR_GLUCOSE_AMOUNT_RAW { get; set; }

        public int PREDICTED_SENSOR_GLUCOSE_AMOUNT
        {
            get
            {
                return ((SgvRaw1 & 3) << 8) | PREDICTED_SENSOR_GLUCOSE_AMOUNT_RAW;
            }
        }

        [BinaryElement(1, Length = 1)]
        public byte SgvRaw1 { get; set; }

        [BinaryElement(2, Length = 1)]
        public byte SgvRaw2 { get; set; }


        public int Amount
        {
            get
            {
                return ((SgvRaw1 & 3) << 8) | SgvRaw2;
            }
        }

        //[BinaryElement(3, Length = 2)]
        //public Int16 VCNTR { get; set; }

        [BinaryElement(3, Length = 2)]
        public Int16 IsigRaw { get; set; }

        public double Isig { get { return (double)this.IsigRaw / 100; } }

        [BinaryElement(5, Length = 1)]
        public byte Uknown2 { get; set; }

        [BinaryElement(6, Length = 2)]
        public Int16 RateOfChangeRaw { get; set; }

        public double RateOfChange { get { return (double)this.RateOfChangeRaw / 100; } }

        public SgvTrend Trend
        {
            get

            {
                if (Amount<=400)
                {
                    //maybe there is a max
                    if (RateOfChangeRaw > 300)
                    {
                        return SgvTrend.DoubleUp;
                    }
                    if (RateOfChangeRaw <= 300 && RateOfChangeRaw >= 100 )
                    {
                        return SgvTrend.SingleUp;
                    }

                    if (RateOfChangeRaw <= 101 && RateOfChangeRaw >= 51)
                    {
                        return SgvTrend.FortyFiveUp;
                    }


                    if (RateOfChangeRaw <= 50 && RateOfChangeRaw >= -50 )
                    {
                        return SgvTrend.Flat;
                    }

                    if (RateOfChangeRaw <= -51 && RateOfChangeRaw >= -100)
                    {
                        return SgvTrend.FortyFiveDown;
                    }
                    if (RateOfChangeRaw <= -101 && RateOfChangeRaw >= -300 )
                    {
                        return SgvTrend.SingleDown;
                    }
                    //maybe there is a max
                    if (RateOfChangeRaw < -300)
                    {
                        return SgvTrend.DoubleDown;
                    }
                }
                return SgvTrend.NotComputable;

               

            }
        }

        [BinaryElement(8, Length = 1)]
        public byte SensorStatus { get; set; }

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
            return $"{Amount}/{Isig}/{RateOfChange}/{PREDICTED_SENSOR_GLUCOSE_AMOUNT_RAW}/{Trend}";
        }
    }
}
