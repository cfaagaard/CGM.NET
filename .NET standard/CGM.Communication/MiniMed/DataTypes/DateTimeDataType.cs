using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{
    public class DateTimeDataType : BaseDataType
    {
        [BinaryElement(0, Length = 4)]
        public Int32 Rtc { get; set; }

        [BinaryElement(4, Length = 4)]
        public Int32 Offset { get; set; }


        public string DateTimeString { get { return DateTime.Value.ToString("ddd, MMM dd HH:mm:ss CEST yyyy", CultureInfo.InvariantCulture); } }

        public DateTime? DateTime
        {
            get
            {
                if (Rtc!=0 && Offset!=0)
                {
                    return DateTimeExtension.GetDateTime(this.Rtc, this.Offset);
                }
                return null;
            }
        }

        public double DateTimeEpoch
        {
            get
            {
                if (this.DateTime.HasValue)
                {
                    DateTimeOffset utcTime2 = this.DateTime.Value;
                    return utcTime2.ToUnixTimeMilliseconds();
                }
                return 0;
            }
        }

        public DateTimeDataType(int rtc,int offset)
        {
            this.Rtc = rtc;
            this.Offset = offset;
        }

        public DateTimeDataType(byte[] rtc, byte[] offset)
        {
            this.Rtc = rtc.GetInt32BigE(0);
            this.Offset = offset.GetInt32BigE(0);
        }

        public DateTimeDataType()
        {

        }
        public override string ToString()
        {
            if (DateTime.HasValue)
            {
                return DateTime.Value.ToString();
            }
            return "(NoDate)";
        }

    }
}
