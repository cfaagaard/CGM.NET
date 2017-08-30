using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CGM.Communication.Extensions
{
    public static class DateTimeExtension
    {
        public const long max = 0x00000000ffffffffL;
        public const long baseTime = 946684800;

        public static byte[] GetRtcBytes(this DateTime datetime, long offset)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var SecondsFromEpoch = datetime.Subtract(epoch).TotalSeconds;

            var rtc = SecondsFromEpoch  - offset -max-baseTime;
            var bytes = BitConverter.GetBytes((int)rtc);
            return bytes;

            //long baseTime = 946684800;

            //DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;
            //DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

            //TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            //TimeSpan sp_offset0 = new TimeSpan(offset * TimeSpan.TicksPerSecond);
            //TimeSpan sp_dateOffsetUtc = new TimeSpan(nowUtc.Millisecond * TimeSpan.TicksPerMillisecond);

            //var datetest2 = datetime.Ticks - baseDateTime.Ticks - sp_offset0.Ticks + sp_dateOffsetUtc.Ticks;

            //TimeSpan sp_baseTime2 = new TimeSpan(datetest2);
            //var seconds = sp_baseTime2.TotalSeconds - 0x00000000ffffffffL;
            //var bytes = BitConverter.GetBytes((int)seconds).Reverse();

            //return bytes.ToArray();
        }

        public static DateTime? GetDateTime(byte[] Rtc, byte[] offSet)
        {
            var rtc = Rtc.GetUInt32(0);
            var offset = offSet.GetUInt32(0);
            return GetDateTime(rtc, offset);
        }

        public static DateTime? GetDateTime(uint rtc, uint offset)
        {
            if (rtc == 0)
            {
                return null;
            }

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);

            var SecondsFromEpoch = baseTime + rtc + offset + max;
            TimeSpan timeSpan = new TimeSpan(SecondsFromEpoch * TimeSpan.TicksPerSecond);
            var result = epoch.Add(timeSpan);
            return result;

            //DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;
            //DateTimeOffset nowUtc = DateTimeOffset.UtcNow;



            //TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            //TimeSpan sp_rtc = new TimeSpan(rtc2 * TimeSpan.TicksPerSecond);
            //TimeSpan sp_offset = new TimeSpan(offSet * TimeSpan.TicksPerSecond);
            //TimeSpan sp_dateOffsetUtc = new TimeSpan(nowUtc.Millisecond * TimeSpan.TicksPerMillisecond);

            //var dateOffset = baseDateTime.Add(sp_rtc);
            //dateOffset = dateOffset.Add(sp_offset);
            //dateOffset = dateOffset.Subtract(sp_dateOffsetUtc);
            //return dateOffset.DateTime;
        }
    }
}
