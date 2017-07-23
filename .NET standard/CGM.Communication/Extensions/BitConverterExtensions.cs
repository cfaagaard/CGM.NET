using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.Extensions
{
    public static class BitConverterExtensions
    {

        public static UInt16 GetUInt16BigE(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToUInt16(bytes.Skip(startIndex).Take(2).Reverse().ToArray(), 0);
        }

        public static UInt32 GetUInt32BigE(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToUInt32(bytes.Skip(startIndex).Take(4).Reverse().ToArray(), 0);
        }

        public static Int16 GetInt16BigE(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt16(bytes.Skip(startIndex).Take(2).Reverse().ToArray(), 0);
        }

        public static Int32 GetInt32BigE(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt32(bytes.Skip(startIndex).Take(4).Reverse().ToArray(), 0);
        }

        public static Int64 GetInt64BigE(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt64(bytes.Skip(startIndex).Take(8).Reverse().ToArray(), 0);
        }


        public static UInt16 GetUInt16(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToUInt16(bytes, startIndex);
        }

        public static UInt32 GetUInt32(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToUInt32(bytes, startIndex);
        }

        public static Int16 GetInt16(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt16(bytes, startIndex);
        }

        public static Int32 GetInt32(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt32(bytes, startIndex);
        }

        public static Int64 GetInt64(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToInt64(bytes, startIndex);
        }
    }
}
