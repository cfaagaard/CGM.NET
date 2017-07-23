using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Requests;

namespace CGM.Communication.MiniMed.Requests
{
    [BinaryType]
    public class RadioChannelRequest : MedtronicMessage3
    {
        [BinaryElement(2)]
        public byte Unknown4 { get; set; }

        [BinaryElement(3)]
        public byte RadioChannel { get; set; }

        [BinaryElement(4)]
        public byte[] Unknown5 { get; set; }

        [BinaryElement(12)]
        public byte[] LinkMac { get; set; }

        [BinaryElement(20)]
        public byte[] PumpMac { get; set; }

        [BinaryElement(28)]
       [BinaryElementLogicCrc16Ciit]
        public byte[] Crc16citt { get; set; }


        public RadioChannelRequest()
        {

        }

        public RadioChannelRequest(byte radioChannel, byte[] linkMac, byte[] pumpMac) : base(AstmCommandAction.CHANNEL_NEGOTIATE)
        {
            this.Unknown4 = 0x01;
            this.RadioChannel = radioChannel;
            this.Unknown5 = new byte[] { 0x00, 0x00, 0x00, 0x07, 0x07, 0x00, 0x00, 0x02 };
            this.LinkMac = linkMac;
            this.PumpMac = pumpMac;
            Crc16citt = new byte[] { 0x00, 0x00 };
        }
    }

}
