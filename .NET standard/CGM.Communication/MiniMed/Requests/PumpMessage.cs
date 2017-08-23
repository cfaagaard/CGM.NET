using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Requests
{

    [BinaryType(IsLittleEndian = false, IsEncrypted = true)]
    public class PumpMessage : IBinaryType
    {

        [BinaryElement(0)]
        public byte Prefix { get; set; }

        [BinaryElement(1)]
        public byte[] MessageType { get; set; }

        [BinaryElement(3)]
        [MessageType(typeof(ReadHistoryInfoRequest),nameof(MessageTypeName), AstmSendMessageType.READ_HISTORY_INFO_REQUEST)]
        [MessageType(typeof(ReadHistoryRequest), nameof(MessageTypeName), AstmSendMessageType.READ_HISTORY_REQUEST)]
        [MessageType(typeof(MissingSegmentRequest), nameof(MessageTypeName), AstmSendMessageType.MULTIPACKET_RESEND_PACKETS)]
        [MessageType(typeof(PumpGeneral))]
        public object Message { get; set; }

        [BinaryElement(1000, Length = 2)]
        [BinaryElementLogicCrc16Ciit]
        public byte[] Crc16citt { get; set; }

        public PumpMessage()
        {

        }

        public PumpMessage(byte prefix, byte[] messageType, byte[] message)
        {
            this.Prefix = prefix;
            this.MessageType = messageType;
            this.Message = message;
            this.Crc16citt = new byte[] { 0x00, 0x00 };
        }
        public PumpMessage(byte prefix, AstmSendMessageType messageType, byte[] message) : this(prefix, BitConverter.GetBytes((short)messageType), message)
        {

        }
        public AstmSendMessageType MessageTypeName
        {
            get
            {
                return (AstmSendMessageType)MessageType.GetUInt16(0);
            }
        }

        public override string ToString()
        {
            return $"{this.MessageTypeName.ToString()} - {this.Message?.ToString()}";
        }


    }
}
