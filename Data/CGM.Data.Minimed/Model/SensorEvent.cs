using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class SensorEvent:BaseEvent
    {
        public int SensorEventId { get; set; }

        public ICollection<SensorReading> SensorReadings { get; set; }

        public SensorEvent()
        {
            SensorReadings = new HashSet<SensorReading>();
        }
    }
}
