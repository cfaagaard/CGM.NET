using CGM.Communication.Common;
using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Nightscout
{
    [Serializable]
    public class NotifiyConfiguration:IConfiguration
    {
      
        public string NotificationUrl { get; set; }
    }
}
