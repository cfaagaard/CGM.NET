using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Extensions
{
    public static class DoubleExtensions
    {
        public static DateTime GetDateTime(this double milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(milliseconds));
        }
    }
}
