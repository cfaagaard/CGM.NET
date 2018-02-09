using CGM.Communication.Extensions;
using CGM.Communication.MiniMed;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common
{
    public class Setting
    {

        public int SettingId { get; set; } = 1;



        public bool UploadToNightscout { get; set; } = true;
        public string NightscoutUrl { get; set; }
        public string NightscoutApiUrl { get { return $"{this.NightscoutUrl}/api/v1"; } }
        public bool HandleAlert776 { get; set; } = true;
        public string NightscoutSecretkey { get; set; }
        public string ApiKeyHashed
        {
            get
            {
                if (!string.IsNullOrEmpty(this.NightscoutSecretkey))
                {
                    return this.NightscoutSecretkey.Sha1Digest();
                }
                return "";
            }
        }

        public bool SendEventsToNotificationUrl { get; set; } = false;
        public string NotificationUrl { get; set; }

        public int DatabaseVersion { get; set; } = 0;

        public int IntervalSeconds { get; set; } = 300;

        public int TimeoutSeconds { get; set; } = 5;
        public bool IncludeHistory { get; set; } = true;
        public int HistoryDaysBack { get; set; } = 7;

        public bool OnlyFromTheLastReading { get; set; } = false;

        public bool AutoStartTask { get; set; } = true;


        public List<LastPumpRead> LastRead { get; set; } = new List<LastPumpRead>();







    }
}
