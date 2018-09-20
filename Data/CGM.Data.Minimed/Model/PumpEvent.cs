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

        public override string ToString()
        {
            return Title;
        }
    }
}
