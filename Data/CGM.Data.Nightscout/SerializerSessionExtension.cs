using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.Interfaces;

namespace CGM.Data.Nightscout
{
    public static class SerializerSessionExtension
    {
        public static NightscoutConfiguration NightscoutConfiguration(this SerializerSession session)
        {
            return session.GetConfiguration<NightscoutConfiguration>();
        }
        public static NotifiyConfiguration NotifiyConfiguration(this SerializerSession session)
        {
            return session.GetConfiguration<NotifiyConfiguration>();
        }

    }
}
