using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.DTO
{
    [Serializable]
    public class SessionDTO
    {
        public byte[] PumpTime { get; set; }
        public List<byte[]> PumpEvents { get; set; } = new List<byte[]>();
        public List<byte[]> SensorEvents { get; set; } = new List<byte[]>();

        public byte[] BolusWizardBGTargets { get; set; }
        public byte[] BolusWizardSensitivityFactors { get; set; }
        public byte[] CarbRatio { get; set; }
        public byte[] DeviceCharacteristics { get; set; }
        public List<byte[]> PumpPatterns { get; set; } = new List<byte[]>();

        public byte[] Device { get; set; }

        public List<byte[]> Status { get; set; } = new List<byte[]>();
    }


}
