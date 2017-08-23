using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGM.Communication.Extensions;
namespace CGM.Communication.Test.LogFiles
{
    public class LogFileReader
    {
        private SerializerSession _session;


        public SerializerSession Session { get { return _session; } }
        public ByteMessageCollection Messages { get; set; }


        public LogFileReader(string path, SerializerSession settings)
        {
            _session = settings;
            Messages = new ByteMessageCollection(_session);
            var lines = System.IO.File.ReadAllLines(path);
            int nr = 1;
            //6/1/2017 12:02:23 AM; 00-00-00-00-01-58
            foreach (var item in lines)
            {
                var split = item.Split(';');
                if (split.Length == 2)
                {
                    string bytestr = split[1].Trim();
                    if (bytestr.StartsWith("00-"))
                    {
                        Messages.Add(bytestr.GetBytes(), nr, split[0]);
                        nr += 1;
                    }

                }

            }
        }
    }
}
