using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Extensions;
using System;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false, IsEncrypted = true)]
    public class PumpMessageStartResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte Prefix { get; set; }

        [BinaryElement(1, Length = 2)]
        public byte[] MessageType { get; set; }

        public AstmSendMessageType MessageTypeName
        {
            get
            {

                    return (AstmSendMessageType)this.MessageType.GetUInt16(0);
                              
            }
        }

        [BinaryElement(3)]
        [MessageType(typeof(PumpTimeMessage), nameof(MessageTypeName), AstmSendMessageType.TIME_RESPONSE,LengthEquals =13)]
        [MessageType(typeof(PumpStatusMessage), nameof(MessageTypeName), AstmSendMessageType.READ_PUMP_STATUS_RESPONSE)]
        [MessageType(typeof(PumpPattern), nameof(MessageTypeName), AstmSendMessageType.READ_BASAL_PATTERN_RESPONSE)]
        [MessageType(typeof(Packet), nameof(MessageTypeName), AstmSendMessageType.MULTIPACKET_SEGMENT_TRANSMISSION)]
        [MessageType(typeof(InitiateMultiPacketTransferResponse), nameof(MessageTypeName), AstmSendMessageType.INITIATE_MULTIPACKET_TRANSFER)]
        [MessageType(typeof(PumpStateHistoryReadInfoResponse), nameof(MessageTypeName), AstmSendMessageType.READ_HISTORY_INFO_RESPONSE)]
        [MessageType(typeof(PumpCarbRatioResponse), nameof(MessageTypeName), AstmSendMessageType.READ_BOLUS_WIZARD_CARB_RATIOS_RESPONSE)]
        [MessageType(typeof(EndHistoryTransmissionResponse), nameof(MessageTypeName), AstmSendMessageType.END_HISTORY_TRANSMISSION)]
        [MessageType(typeof(HighSpeedModeResponse), nameof(MessageTypeName), AstmSendMessageType.HIGH_SPEED_MODE_COMMAND)]
        [MessageType(typeof(DeviceCharacteristicsResponse), nameof(MessageTypeName), AstmSendMessageType.DEVICE_CHARACTERISTICS_RESPONSE)]
        [MessageType(typeof(DeviceStringResponse), nameof(MessageTypeName), AstmSendMessageType.DEVICE_STRING_RESPONSE)]
        [MessageType(typeof(BolusWizardBGTargetsResponse), nameof(MessageTypeName), AstmSendMessageType.READ_BOLUS_WIZARD_BG_TARGETS_RESPONSE)]
        [MessageType(typeof(BolusWizardSensitivityFactorsResponse), nameof(MessageTypeName), AstmSendMessageType.READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_RESPONSE)]

        
        [MessageType(typeof(PumpGeneral))]
        public object Message { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            if (Message is PumpGeneral)
            {
                settings.GeneralMessages.Add(this);
            }
            //if (Message is PumpStateHistory)
            //{
            //    settings.PumpStateHistory.Add(this);
            //}
        }

        public override string ToString()
        {
            return this.MessageTypeName.ToString() + "-" + Message.ToString();
        }
    }

    //[BinaryType]
    //public class GeneralResponse : IBinaryType, IBinaryDeserializationCallback
    //{
    //    [BinaryElement(0)]
    //    public BayerCommand BayerCommand { get; set; }

    //    public AstmCommandType CommandTypeName { get; set; }

    //    public AstmCommandAction CommandActionName { get; set; }

    //    public void OnDeserialization(byte[] bytes)
    //    {

    //    }
    //}
}
