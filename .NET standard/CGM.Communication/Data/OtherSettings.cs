using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data
{
    public class OtherSettings
    {
        public int IntervalSeconds { get; set; } = 300;
        public bool IncludeHistory { get; set; } = true;
        public bool SendEventsToNotificationUrl { get; set; } = false;


        public int HistoryDaysBack { get; set; } = 7;
        public string TurnOffIfOnThisWifi { get; set; }

        public bool OnlyFromTheLastReading { get; set; } = false;

        public bool AutoStartTask { get; set; } = true;

        public bool UploadToNightscout { get; set; } = true;

        public int TimeoutSeconds { get; set; } = 5;

        public List<LastPumpRead> LastRead { get; set; } = new List<LastPumpRead>();
    }

    public class LastPumpRead
    {
        public int DataType { get; set; }
        public int LastRtc { get; set; }
    }
}
