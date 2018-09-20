using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class TEMP_BASAL_PROGRAMMED_Event : BaseEvent
    {

        //int preset = read8toUInt(eventData, index + 0x0B);

        //int type = read8toUInt(eventData, index + 0x0C);

        //double rate = read32BEtoInt(eventData, index + 0x0D) / 10000.0;

        //int percentageOfRate = read8toUInt(eventData, index + 0x11);

        //int duration = read16BEtoUInt(eventData, index + 0x12);

        //[BinaryElement(0)]
        //public Int32 RateRaw { get; set; }

        //public Int32 Rate { get { return RateRaw / 10000; } }

        [BinaryElement(5)] 
        public Int16 Percentage { get; set; }

        [BinaryElement(7)] 
        public Int16 Duration { get; set; }


        public override string ToString()
        {
            return $"{this.EventDate.DateTime.Value} - {this.Percentage}% for {this.Duration} minutes)";
        }
    }
}
