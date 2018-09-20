using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class HighSpeedModeResponse : IBinaryType, IBinaryDeserializationSetting
    {
        //maybe status. It seems 0=bad, 1=good....
        [BinaryElement(0)]
        public byte Unknown1 { get; set; }

        [BinaryElement(1)]
        public byte Unknown2 { get; set; }
        [BinaryElement(2)]
        public byte Unknown3 { get; set; }
        [BinaryElement(3)]
        public byte Unknown4 { get; set; }
        [BinaryElement(4)]
        public byte Unknown5 { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
        
        }
    }
}
