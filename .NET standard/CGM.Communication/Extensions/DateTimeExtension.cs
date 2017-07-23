using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CGM.Communication.Extensions
{
    public static class DateTimeExtension
    {


        public static byte[] GetRtcBytes(this DateTime datetime, long offset)
        {

            long baseTime = 946684800;

            DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

            TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            TimeSpan sp_offset0 = new TimeSpan(offset * TimeSpan.TicksPerSecond);
            TimeSpan sp_dateOffsetUtc = new TimeSpan(nowUtc.Millisecond * TimeSpan.TicksPerMillisecond);

            var datetest2 = datetime.Ticks - baseDateTime.Ticks - sp_offset0.Ticks + sp_dateOffsetUtc.Ticks;

            TimeSpan sp_baseTime2 = new TimeSpan(datetest2);
            var seconds = sp_baseTime2.TotalSeconds - 0x00000000ffffffffL;
            var bytes = BitConverter.GetBytes((int)seconds).Reverse();

            return bytes.ToArray();
        }

        public static DateTime? GetDateTime(byte[] Rtc, byte[] offSet)
        {
            var rtc = Rtc.GetInt32(0);
            var offset = offSet.GetInt32(0);
            return GetDateTime(rtc, offset);
        }

        public static DateTime? GetDateTime(int Rtc, int offSet)
        {
            if (Rtc==0)
            {
                return null;
            }
            long baseTime = 946684800;
            long rtc2 = Rtc & 0x00000000ffffffffL;

            DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

            TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            TimeSpan sp_rtc = new TimeSpan(rtc2 * TimeSpan.TicksPerSecond);
            TimeSpan sp_offset = new TimeSpan(offSet * TimeSpan.TicksPerSecond);
            TimeSpan sp_dateOffsetUtc = new TimeSpan(nowUtc.Millisecond * TimeSpan.TicksPerMillisecond);

            var dateOffset = baseDateTime.Add(sp_rtc);
            dateOffset = dateOffset.Add(sp_offset);
            dateOffset = dateOffset.Subtract(sp_dateOffsetUtc);
            return dateOffset.DateTime;
        }
    }
}
