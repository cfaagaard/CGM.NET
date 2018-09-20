using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CGM.Communication.Extensions
{
    public static class SerializerSessionExtension
    {
        public static MinimedConfiguration MinimedConfiguration(this SerializerSession session)
        {
           return session.GetConfiguration<MinimedConfiguration>();
        }

        public static PumpSettings PumpSettings(this SerializerSession session)
        {
            return session.GetConfiguration<PumpSettings>();
        }
    }
}
