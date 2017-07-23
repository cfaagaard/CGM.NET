using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using System.Linq;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian=false)]
    public class PumpStateHistory : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0, Length = 2)]
        public byte[] SeqNumber { get; set; }

        [BinaryElement(2)]
        public byte[] Unknown1 { get; set; }

        //[BinaryElement(99)]
        //public byte[] Unknown2 { get; set; }

        public byte[] AllBytes { get; set; }
        public byte[] AllBytesE { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //var list = new List<byte>();
            //list.AddRange(bytes);
            //this.AllBytes = list.GetRange(3,list.Count-3).ToArray();


            this.AllBytes = bytes;
            this.AllBytesE = bytes.Reverse().ToArray();
            this.BytesAsString = BitConverter.ToString(AllBytes);

            settings.PumpDataHistory.PumpStateHistory.Add(this);
        }

        public override string ToString()
        {
            return $"{BitConverter.ToString(this.SeqNumber)}";
        }
    }

}
