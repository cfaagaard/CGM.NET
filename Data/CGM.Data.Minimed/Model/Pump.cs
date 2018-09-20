using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class Pump
    {
        public int PumpId { get; set; }

        public string SerialNumber { get; set; }

        public string Mac { get; set; }

        public byte FirmwareMajorNumber { get; set; }

   
        public byte FirmwareMinorNumber { get; set; }

       
        public string FirmwareAlphaNumber { get; set; }

        public string Firmware { get; set; }

 
        public byte MotorMajorNumber { get; set; }

        
        public byte MotorMinorNumber { get; set; }

      
        public string MotorAlphaNumber { get; set; }

        public string Motor { get; set; }

   
        public byte BgUnitRaw { get; set; }

        public BgUnitEnum BgUnit { get { return (BgUnitEnum)BgUnitRaw; } }

        public string BytesAsString { get; set; }

        public ICollection<PumpEvent> PumpEvents { get; set; }
        public ICollection<SensorEvent> SensorEvents { get; set; }
        public ICollection<PumpStatus> PumpStatus { get; set; }

        public Pump()
        {
            PumpStatus = new HashSet<PumpStatus>();
            PumpEvents = new HashSet<PumpEvent>();
            SensorEvents = new HashSet<SensorEvent>();
        }
    }
}
