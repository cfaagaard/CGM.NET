using CGM.Communication.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.DTO
{
    public class DataTransferParameters
    {
        public bool GetPumpSettings { get; set; }
        public LastPumpRead PumpEventLastRead { get; set; }
        public LastPumpRead SensorEventLastRead { get; set; }
    }
}
