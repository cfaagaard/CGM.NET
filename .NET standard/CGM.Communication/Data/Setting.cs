using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data
{
    public class Setting
    {
        [SQLite.PrimaryKey]
        public int SettingId { get; set; } = 1;

        [SQLite.MaxLength(3000)]
        public string NightscoutUrl { get; set; }

        [SQLite.MaxLength(3000)]
        public string NightscoutApiUrl { get { return $"{this.NightscoutUrl}/api/v1"; } }

        [SQLite.MaxLength(50)]
        public string NightscoutSecretkey { get; set; }

        [SQLite.MaxLength(3000)]
        public string NotificationUrl { get; set; }

        public int DatabaseVersion { get; set; } = 0;

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
    }
}
