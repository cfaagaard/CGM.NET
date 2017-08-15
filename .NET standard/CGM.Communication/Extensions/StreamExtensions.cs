using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CGM.Communication.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }

        public static byte[] ToByteArray(this Stream stream,int length)
        {
            stream.Position = 0;
            byte[] buffer = new byte[length];
            for (int totalBytesCopied = 0; totalBytesCopied < length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }
    }
}
