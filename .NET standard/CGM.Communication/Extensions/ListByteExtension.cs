using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.Extensions
{
   public static class ListByteExtension
    {

        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

        public static byte[] JoinToArray(this List<byte[]> bytes)
        {
            if (bytes != null && bytes.Count > 0)
            {
                List<byte> newbytes = new List<byte>();
                newbytes.AddRange(bytes[0]);
                if (bytes.Count > 1)
                {
                    for (int i = 1; i < bytes.Count; i++)
                    {
                        List<byte> newlist = new List<byte>(bytes[i]);


                        var j = newlist.Count - 1;
                        while (newlist[j] == 0)
                        {
                            --j;
                        }
                        var temp = new byte[j + 1];

                        Array.Copy(newlist.ToArray(), temp, j + 1);

                        var list = temp.ToList();
                        list.RemoveRange(0, 5);
                        newbytes.AddRange(list);
                    }
                    newbytes[4] = (byte)(newbytes.Count - 5);
                }
                var data = newbytes.ToArray();
                bool data_found = false;
                byte[] new_data = data.Reverse().SkipWhile(point =>
                {
                    if (data_found) return false;
                    if (point == 0x00) return true; else { data_found = true; return false; }
                }).Reverse().ToArray();

                return new_data;
            }
            return null;
        }

        public static IEnumerable<List<byte>> SplitList(this List<byte> list, int nSize)
        {
            return ListByteExtension.splitList<byte>(list, 60);
        }

        public static IEnumerable<List<T>> splitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}
