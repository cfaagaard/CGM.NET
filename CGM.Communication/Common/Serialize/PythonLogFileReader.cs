using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using System.Linq;

namespace CGM.Communication.Common.Serialize
{

    public class PythonLogFileReader
    {
        private SerializerSession _session;


        public SerializerSession Session { get { return _session; } }
        public ByteMessageCollection Messages { get; set; }

        public PythonLogFileReader( SerializerSession session)
        {
            _session = session;
            Messages = new ByteMessageCollection(_session);
        }

        public PythonLogFileReader(string path, SerializerSession session):this(session)
        {
            var lines = System.IO.File.ReadAllLines(path);
            ReadLines(lines);
        }


        private void ReadLines(string[] lines)
        {
            int nr = 1;
            foreach (var item in lines)
            {
                string bytestr = item;
                //hack for the python-logs....
                if (bytestr.StartsWith("41"))
                {
                    bytestr = "00" + bytestr;
                }
                Messages.Add(bytestr.ToByteArray(), nr, "");
                nr += 1;
            }
        }
    }
}
