using CGM.Communication.Common;
using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed
{
    [Serializable]
    public class MinimedConfiguration : IConfiguration
    {
        public int IntervalSeconds { get; set; } = 300;
        public int TimeoutSeconds { get; set; } = 20;
        public bool IncludePumpSettings { get; set; } = true;
        public int HistoryDaysBack { get; set; } = 2;
        public List<LastPumpRead> LastRead { get; set; } = new List<LastPumpRead>();
        public bool IncludeHistory { get; set; } = true;

    }
}
