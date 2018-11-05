using CGM.Communication.MiniMed.Infrastructur;
using CGM.Data.Minimed.Model.Owned;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class BaseEvent
    {


        public int PumpId { get; set; }

        public virtual Pump Pump { get; set; }

        public int DataLoggerReadingId { get; set; }

        public virtual DataLoggerReading DataLoggerReading { get; set; }

        public string Title { get; set; }

        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }

        public byte Source { get; set; }

        public byte Length { get; set; }

        public DateTimeDataType EventDate { get; set; }


        public string BytesAsString { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
