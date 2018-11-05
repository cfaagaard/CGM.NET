using CGM.Data.Minimed.Model.Owned;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class Calibration
    {
        public int CalibrationID { get; set; }
        public int PumpEventId { get; set; }
        public PumpEvent PumpEvent { get; set; }
        public UInt16 CalibrationFactor { get; set; }

 
        public BgDataType BG { get; set; }
    }
}
