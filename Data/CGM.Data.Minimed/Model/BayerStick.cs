using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class BayerStick
    {
        public int BayerStickId { get; set; }
        public string Value { get; set; }

        public string SerialNumberFull { get; set; }

        public string RFID { get; set; }

        public string ModelNumber { get; set; }

        public string SerialNumber { get; set; }

        public string HMACbyte { get; set; }

        public string DigitalEngineVersion { get; set; }
        public string AnalogEngineVersion { get; set; }
        public string GameBoardVersion { get; set; }

        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public string SerialNum { get; set; }
        public string SerialNumSmall { get; set; }
        public string SkuIdentifier { get; set; }
        public string AccessPassword { get; set; }
        public string MeterLanguage { get; set; }
        public string TestReminderInterval { get; set; }

   
        public string AllBytesAsString { get; set; }

        public ICollection<DataLoggerReading> DataLoggerReadings { get; set; }

        public BayerStick()
        {
            DataLoggerReadings = new HashSet<DataLoggerReading>();
        }
    }
}
