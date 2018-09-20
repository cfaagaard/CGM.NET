using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class BaseEvent
    {


        public int PumpId { get; set; }

        public virtual Pump Pump { get; set; }

        public int BayerStickId { get; set; }

        public virtual BayerStick BayerStick { get; set; }

        public string Title { get; set; }

        public byte EventTypeRaw { get; set; }

        public EventTypeEnum EventType
        {
            get
            {
                return (EventTypeEnum)EventTypeRaw;

            }

        }

        public byte Source { get; set; }

        public byte Length { get; set; }

        public DateTime EventDate { get; set; }

        public Int32 Rtc { get; set; }

        public Int32 Offset { get; set; }

        public string BytesAsString { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
