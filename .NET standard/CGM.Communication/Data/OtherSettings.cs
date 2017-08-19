using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data
{
    public class OtherSettings
    {
        public int IntervalSeconds { get; set; } = 300;
        public bool IncludeHistory { get; set; } = true;
        public bool SendEventsToNotificationUrl { get; set; } = true;


        public int HistoryDaysBack { get; set; } = 1;
        public string TurnOffIfOnThisWifi { get; set; }

        public bool OnlyFromTheLastReading { get; set; }

        public bool AutoStartTask { get; set; } = true;
    }
}
