
using CGM.Communication;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Patterns
{

    public class ReportPattern: IReportPattern
    {
        public byte[] Bytes { get; set; }
        public int StartPosition { get; set; }
        public int Length { get; set; }
        protected ILogger Logger = ApplicationLogging.CreateLogger<ReportPattern>();
        //public ReportPattern()
        //{

        //}
        public ReportPattern(byte b) : this(b, 0)
        {

        }
        public ReportPattern(byte b, int startPosition):this(new byte[] {b },startPosition)
        {
 
        }

        public ReportPattern(byte b, int startPosition, int length) : this(new byte[] { b }, startPosition)
        {

        }

        public ReportPattern(byte[] bytes) : this(bytes, 0)
        {

        }
        public ReportPattern(byte[] bytes, int startPosition):this(bytes,startPosition,bytes.Length)
        {
        }
            public ReportPattern(byte[] bytes, int startPosition, int length)
        {
            this.Bytes = bytes;
            this.StartPosition = startPosition;
            if (bytes.Length<length)
            {
                length = bytes.Length;
            }
            this.Length = length;
            
        }

        public int FindIndex(byte[] reportBytes)
        {
            int j = 0;
            int index = -1;
            for (int i = StartPosition; i < (Bytes.Length + StartPosition); i++)
            {
                if (reportBytes[i] == Bytes[j])
                {
                    index = i;
                    break;
                }
                j += 1;
            }
            return index;
        }


        public bool Evaluate(byte[] reportBytes)
        {
            int j = 0;
            bool match = false;
            for (int i = StartPosition; i < (Length + StartPosition); i++)
            {
                    if (reportBytes[i] != Bytes[j])
                    {
                        match = false;
                        break;
                    }
                    else
                    {
                        match = true;
                    }
                j += 1;
                }
            return match;
        }
    }

    
}
