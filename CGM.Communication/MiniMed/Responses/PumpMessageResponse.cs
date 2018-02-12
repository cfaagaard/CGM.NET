using CGM.Communication.Common.Serialize;

namespace CGM.Communication.MiniMed.Responses
{
    public class PumpMessageResponse : PumpResponse
    {
        [BinaryElement(4)]
        public byte[] PumpMac { get; set; }
        [BinaryElement(12)]
        public byte[] LinkMac { get; set; }

        [BinaryElement(20)]
        public byte SequenceNumber { get; set; }

        [BinaryElement(21)]
        public byte EncryptedMessageType_maybe { get; set; }

        [BinaryElement(22)]
        public byte Unknown5 { get; set; }

        [BinaryElement(23)]
        public byte PumpMessageLength { get; set; }


        [BinaryElement(24)]
        public PumpMessageStartResponse Message { get; set; }


        public override string ToString()
        {
            return this.GetType().Name.ToString() + " - " + this.Message.ToString();
        }
    }
}
