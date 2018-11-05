using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class SensorReadingAlert
    {
        public int SensorReadingAlertId { get; set; }
        public string SensorReadingAlertName { get; set; }
        public string SensorReadingAlertFullName { get; set; }

        public ICollection<SensorReading> SensorReadings { get; set; }

        public SensorReadingAlert()
        {
            SensorReadings = new HashSet<SensorReading>();
        }
    }
}
