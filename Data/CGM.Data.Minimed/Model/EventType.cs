using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
   public class EventType
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string EventTypeFullName { get; set; }

        public ICollection<PumpEvent> PumpEvents { get; set; }
        public ICollection<SensorEvent> SensorEvents { get; set; }
        public EventType()
        {
            PumpEvents = new HashSet<PumpEvent>();
            SensorEvents = new HashSet<SensorEvent>();
        }
    }
}
