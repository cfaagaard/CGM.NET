using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Requests
{
    [BinaryType]
    public class PumpEnvelope : MedtronicMessage3
    {

        [BinaryElement(2)]
        public byte[] PumpMac { get; set; }

        [BinaryElement(10)]
        public byte SequenceNumber { get; set; }

        [BinaryElement(11)]
        public byte Unknown4 { get; set; }

        [BinaryElement(12)]
        [BinaryElementLogicLength(From =13,Add =-2)]
        public byte PumpMessageLength { get; set; }

        [BinaryElement(13)]
        public PumpMessage Message { get; set; }

        [BinaryElement(1000, Length = 2)]
        [BinaryElementLogicCrc16Ciit]
        public byte[] Crc16citt { get; set; }

        public PumpEnvelope()
        {

        }

        public PumpEnvelope(byte[] pumpMac, byte sequenceNumber) : base(AstmCommandAction.TRANSMIT_PACKET)
        {
            this.PumpMac = pumpMac;
            this.SequenceNumber = sequenceNumber;
            this.Unknown4 = 0x10;
            this.Crc16citt = new byte[] { 0x00, 0x00 };
        }



        public override string ToString()
        {
            if (this.Message!=null)
            {
                return $"{this.Message.ToString()} ({this.SequenceNumber.ToString()})";
            }
            return "";
        }

    }

}
