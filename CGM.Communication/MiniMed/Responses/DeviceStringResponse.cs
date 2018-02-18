using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class DeviceStringResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0,Length =8)]
        public byte[] Mac { get; set; }

        [BinaryElement(8)]
        public Int16 StringType { get; set; }

        [BinaryElement(10)]
        public byte Language { get; set; }

        [BinaryElement(11,Length =16)]
        public byte[] ModelRaw { get; set; }

        public string Model { get { return Encoding.ASCII.GetString(ModelRaw.Reverse().ToArray()).Replace("\0",""); } }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.PumpSettings.DeviceString = this;
        }
    }
}
