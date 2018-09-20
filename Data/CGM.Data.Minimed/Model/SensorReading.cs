using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class SensorReading
    {
        public int SensorReadingId { get; set; }

        public int SensorEventId { get; set; }

        public SensorEvent SensorEvent { get; set; }

        public DateTime ReadingDateTime { get; set; }

        public byte SgvRaw1 { get; set; }

        public byte SgvRaw2 { get; set; }

        public int Amount { get; set; }

        public Int16 IsigRaw { get; set; }

        public double Isig
        {
            get
            {
                return (double)this.IsigRaw / 100;

            }
        }

        public Int16 RateOfChangeRaw { get; set; }

        public double RateOfChange
        {
            get
            {
                return (double)this.RateOfChangeRaw / 100;

            }

        }

        public SgvAlert? Alert
        {
            get
            {
                if (this.Amount > 700)
                {
                    return (SgvAlert)this.Amount;
                }
                return null;
   
            }

        }

        public byte SensorStatus { get; set; }

        public UInt16 PredictedSg { get; set; }



    }
}
