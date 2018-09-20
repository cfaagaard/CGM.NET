using CGM.Communication.Common.Serialize;
using System;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    public class OpenConnectionResponse : PumpResponse
    {
        [BinaryElement(4)]
        public byte[] Unknown2 { get; set; }

        [BinaryElement(9)]
        public byte Unknown3 { get; set; }

        [BinaryElement(10)]
        public byte[] Unknown4 { get; set; }

        [BinaryElement(18)]
        public byte[] Unknown5 { get; set; }

        [BinaryElement(27)]
        public byte[] Crc16citt { get; set; }

        public override string ToString()
        {
            return this.GetType().Name.ToString();
        }
    }
}
