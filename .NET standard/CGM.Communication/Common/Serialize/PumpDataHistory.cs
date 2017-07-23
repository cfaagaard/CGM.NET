using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    public class PumpDataHistory
    {
        public PumpStateHistoryReadInfoResponse ReadInfoResponse { get; set; }
        public List<PumpStateHistory> PumpStateHistory { get; set; } = new List<PumpStateHistory>();

        public override string ToString()
        {
            if (this.ReadInfoResponse!=null)
            {
                return $"{this.ReadInfoResponse.ToString()} ({this.PumpStateHistory.Count})";
            }
            return "(No dates)";
        }
    }
}
