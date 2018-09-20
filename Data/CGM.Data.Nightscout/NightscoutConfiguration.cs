using CGM.Communication.Common;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Nightscout
{
    [Serializable]
    public class NightscoutConfiguration:IConfiguration
    {
     
        public string NightscoutUrl { get; set; }
        [JsonIgnore]
        public string NightscoutApiUrl { get { return $"{this.NightscoutUrl}/api/v1"; } }

        public string NightscoutSecretkey { get; set; }

        [JsonIgnore]
        public bool HandleAlert776 { get; set; } = true;

        [JsonIgnore]
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
