using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGM.Communication.Extensions;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class PumpStateHistoryStart : IBinaryType, IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public Int16 Start { get; set; }

        [BinaryElement(2)]
        public byte DataType { get; set; }

        [BinaryElement(3)]
        public Int32 historySizeCompressed { get; set; }

        [BinaryElement(7)]
        public Int32 historySizeUncompressed { get; set; }

        [BinaryElement(11)]
        public byte historyCompressed { get; set; }


        [BinaryElement(12)]
        public byte[] Rest { get; set; }
  
        public byte[] AllBytes { get; set; }

        public byte[] AllBytesE { get; set; }

        public byte[] AllBytesNoHeader { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            List<byte> list = bytes.ToList();//.GetRange(11,bytes.Length-11);

            this.AllBytes = list.ToArray();
            this.AllBytesE = list.ToArray().Reverse().ToArray();
            //throw new NotImplementedException();
            if (bytes.Length>=12)
            {
                this.AllBytesNoHeader = bytes.ToList().GetRange(12, bytes.Length - 12).ToArray();
            }

       
        }
    }
}
