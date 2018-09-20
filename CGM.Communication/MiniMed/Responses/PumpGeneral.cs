using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class PumpGeneral : IBinaryType, IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public byte[] Unknown1 { get; set; }

        public byte[] AllBytes { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.BytesAsString = BitConverter.ToString(AllBytes);

            
        }

        public override string ToString()
        {
            if (Unknown1!=null)
            {
                return $"GeneralPumpMessage - {BytesAsString}";
            }
            return $"GeneralPumpMessage";

        }
    }

}
