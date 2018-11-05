using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class PumpEvent: BaseEvent
    {

        public int PumpEventId { get; set; }

        public int PumpEventAlertId { get; set; }

        public PumpEventAlert PumpEventAlert { get; set; }

        public ICollection<DailyTotal> DailyTotals { get; set; }
        public ICollection<Calibration> Calibrations { get; set; }

        public PumpEvent()
        {
            DailyTotals= new HashSet<DailyTotal>();
            Calibrations = new HashSet<Calibration>();
        }


        public override string ToString()
        {
            return Title;
        }
    }
}
