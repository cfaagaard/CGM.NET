using System;
using CGM.Communication.Common.Serialize;

namespace CGM.Communication.MiniMed.Responses
{
    public class RadioChannelResponse : PumpResponse, IBinaryDeserializationSetting
    {
        [BinaryElement(4)]
        public byte[] PumpMacFirst { get; set; }

        [BinaryElement(9)]
        public byte Unknown5 { get; set; }

        [BinaryElement(10)]
        public byte[] PumpMac { get; set; }

        [BinaryElement(18)]
        public byte[] Unknown6 { get; set; }

        [BinaryElement(27)]
        public byte[] LinkMac { get; set; }

        [BinaryElement(35)]
        public byte SignalStrength { get; set; }

        [BinaryElement(36)]
        public byte[] Unknown7 { get; set; }

        [BinaryElement(43)]
        public byte RadioChannel { get; set; }

        [BinaryElement(44)]
        public byte[] Crc16citt { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            if (bytes.Length > 43)
            {

                settings.RadioChannel = this.RadioChannel;
                settings.RadioSignalStrength = this.SignalStrength;

            }

        }

        public override string ToString()
        {
            return string.Format("{0} (Channel: {1} - SignalStrength {2}%)", this.GetType().Name.ToString(), this.RadioChannel, this.SignalStrength);
        }

    }
}
