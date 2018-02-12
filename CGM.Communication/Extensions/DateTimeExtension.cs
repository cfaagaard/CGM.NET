using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CGM.Communication.Extensions
{
    public static class DateTimeExtension
    {
     
        //DS3231 - chip starts at 1-1-2000
        public const long baseTime = 946684800;
        public const long max3 = 0x100000000;
        public const long max2 = 0xffffffff;

        public static byte[] GetRtcBytes(this DateTime datetime, int offset)
        {
            DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;
            TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            TimeSpan sp_offset = new TimeSpan(offset * TimeSpan.TicksPerSecond);

            var datetest2 = datetime.Ticks - baseDateTime.Ticks - sp_offset.Ticks;
            TimeSpan sp_baseTime2 = new TimeSpan(datetest2);
            var seconds = sp_baseTime2.TotalSeconds - 0x100000000;
            //should we do this here....
            //var bytes = BitConverter.GetBytes((int)seconds).Reverse();

            return BitConverter.GetBytes((int)seconds).ToArray();
        }



        public static byte[] GetRtcBytes(this DateTime datetime, byte[] offSet)
        {
            var offset = offSet.GetInt32(0);
            return DateTimeExtension.GetRtcBytes(datetime, offset);
        }

        public static DateTime? GetDateTime(byte[] datetime)
        {
            return GetDateTime(datetime.GetInt32BigE(0), datetime.GetInt32BigE(4));
        }

        public static DateTime? GetDateTime(byte[] Rtc, byte[] offSet)
        {
            var rtc = Rtc.GetInt32(0);
            var offset = offSet.GetInt32(0);
            return GetDateTime(rtc, offset);
        }


        public static DateTime? GetDateTime(int Rtc, int offSet)
        {
            if (Rtc == 0)
            {
                return null;
            }

            long rtc2 = Rtc & 0x00000000ffffffffL;

            DateTimeOffset baseDateTime = DateTimeOffset.FromUnixTimeSeconds(baseTime).UtcDateTime;

            TimeSpan sp_baseTime = new TimeSpan(baseTime * TimeSpan.TicksPerSecond);
            TimeSpan sp_rtc = new TimeSpan(rtc2 * TimeSpan.TicksPerSecond);
            TimeSpan sp_offset = new TimeSpan(offSet * TimeSpan.TicksPerSecond);

            var dateOffset = baseDateTime.Add(sp_rtc);
            dateOffset = dateOffset.Add(sp_offset);

            return dateOffset.DateTime;
        }
    }
}
