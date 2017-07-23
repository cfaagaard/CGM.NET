using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian =false)]
    public class ReadInfoResponse :  IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte[] LinkMac { get; set; }
        [BinaryElement(8,Length =8)]
        public byte[] PumpMac { get; set; }
        [BinaryElement(16)]
        public byte Unknown { get; set; }
        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.LinkMac = LinkMac;
            settings.PumpMac = PumpMac;

        }
    }
}
