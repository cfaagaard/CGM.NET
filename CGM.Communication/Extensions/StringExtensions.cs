using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.Extensions
{
    public static class StringExtensions
    {
        public static string Right(this string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }

       public static IEnumerable<string> SplitToLines(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static byte[] GetBytes(this string hex)
        {
            string[] strBytes = hex.Split('-');
            List<byte> bytes = new List<byte>();
            foreach (var strbyte in strBytes)
            {
                int value = Convert.ToInt32(strbyte, 16);
                string stringValue = Char.ConvertFromUtf32(value);
                bytes.Add(Convert.ToByte(value));
            }
            return bytes.ToArray();
        }

        public static string Sha1Digest(this string apiKey)
        {
            byte[] key = Encoding.UTF8.GetBytes(apiKey);
            byte[] digestKey = key.Sha1Digest();
            String result3 = BitConverter.ToString(digestKey).Replace("-", "").ToLower();
            return result3;
        }

        //public static string Sha1Digest2(this string apiKey)
        //{
        //    byte[] key = Encoding.UTF8.GetBytes(apiKey);
        //    byte[] digestKey = key.Sha1Digest2();
        //    String result3 = BitConverter.ToString(digestKey).Replace("-", "").ToLower();
        //    return result3;
        //}
    }
}
