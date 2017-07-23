using CGM.Communication.Common.Serialize;
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Patterns;
using System;

namespace CGM.Communication.MiniMed
{


    [BinaryType]
    public class MedtronicMessage2: IBinaryType,IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public string SerialNumber { get; set; }

        [BinaryElement(6)]
        public byte[] Unknown1 { get; set; }

        [BinaryElement(16)]
        public byte CommandType { get; set; }

        public AstmCommandType CommandTypeName { get { return (AstmCommandType)CommandType; } }

        [BinaryElement(17)]
        public byte SessionNumber { get; set; }

        [BinaryElement(18)]
        public byte[] Unknown2 { get; set; } //8 bytes

        [BinaryElement(26)]
        //[BinaryElementLogicLength(From = 38,Add =7)]
        public byte Length { get; set; } = 0x00;

        [BinaryElement(27)]
        public byte[] Unknown3 { get; set; } //3bytes

        [BinaryElement(30)]
        //[BinaryElementLogicByteSum]
        public byte ByteSum { get; set; }


        [BinaryElement(31)]
        [MessageType(typeof(Requests.ConnectionRequest), typeof(Requests.Patterns.OpenConnectionRequestPattern))]
        [MessageType(typeof(Responses.OpenConnectionResponse), typeof(Responses.Patterns.OpenConnectionResponsePattern))]

        [MessageType(typeof(Requests.RadioChannelRequest), typeof(Requests.Patterns.RadioChannelRequestPattern))]
        [MessageType(typeof(Responses.RadioChannelResponse), typeof(Responses.Patterns.RadioChannelResponsePattern))]


        //[MessageType(typeof(Requests.BeginEhsmRequest), typeof(Requests.Patterns.EHSMRequestPattern))]
        //[MessageType(typeof(Requests.PumpMessageGeneral), typeof(Requests.Patterns.PumpGeneralRequestPattern))]
        [MessageType(typeof(Responses.PumpMessageResponse), typeof(Responses.Patterns.EncryptedResponsePattern))]

        [MessageType(typeof(Requests.ConnectionRequest), typeof(Requests.Patterns.CloseConnectionRequestPattern))]
        [MessageType(typeof(Responses.ReadInfoResponse),typeof(Responses.Patterns.ReadInfoResponsePattern))]
        [MessageType(typeof(Responses.LinkKeyResponse), typeof(Responses.Patterns.LinkKeyResponsePattern))]
        //[MessageType(typeof(Requests.PumpMessageGeneral), typeof(Requests.Patterns.PumpHistoryRequestPattern))]


        [MessageType(typeof(Requests.PumpEnvelope), typeof(Requests.Patterns.PumpRequestPattern))]
        
        public object Message { get; set; }

        public byte[] AllBytes { get; set; }

        public MedtronicMessage2()
        {
        }

        public MedtronicMessage2(byte sessionNumber, AstmCommandType command) : this(sessionNumber, (byte)command)
        {
        }
        public MedtronicMessage2(byte sessionNumber, byte commandType)
        {

            this.SerialNumber = "000000";
            this.Unknown1 = Enumerable.Repeat<byte>(0x00, 10).ToArray();
            this.CommandType = commandType;
            this.SessionNumber = sessionNumber;
            this.Unknown2 = Enumerable.Repeat<byte>(0x00, 8).ToArray();

            this.Length = 0x00;
            this.Unknown3 = Enumerable.Repeat<byte>(0x00, 3).ToArray();
            this.ByteSum = 0x00;
        }


        public override string ToString()
        {
            string returnStr = this.CommandTypeName.ToString();
            if (Message != null)
            {
                returnStr += " - " +  Message.ToString();
            }
            return returnStr;

        }

        public static AstmStart GetNew(byte sessionNumber, AstmCommandType commandType)
        {
            return GetNew(sessionNumber, (byte)commandType);
        }
        public static AstmStart GetNew(byte sessionNumber, byte commandType)
        {
            AstmStart astm = new AstmStart(new byte[] { 0x51, 0x03 });
            astm.Message2 = new MedtronicMessage2(sessionNumber, commandType);

            return astm;
        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
        }
    }

    //[BinaryType]
    //public class MedtronicMessage2 : BayerCommand
    //{

    //    [BinaryElement(7)]
    //    public string SerialNumber { get; set; }

    //    [BinaryElement(13)]
    //    public byte[] Unknown1 { get; set; }

    //    [BinaryElement(23)]
    //    public byte CommandType { get; set; }

    //    public AstmCommandType CommandTypeName { get { return (AstmCommandType)CommandType;  } }

    //    [BinaryElement(24)]
    //    public byte SessionNumber { get; set; }

    //    [BinaryElement(25)]
    //    public byte[] Unknown2 { get; set; } //8 bytes

    //    [BinaryElement(33)]
    //    [BinaryElementLogicLength(Add = -4)]
    //    public byte Length { get; set; } = 0x00;

    //    [BinaryElement(34)]
    //    public byte[] Unknown3 { get; set; } //3bytes

    //    [BinaryElement(37)]
    //    [BinaryElementLogicByteSum]
    //    public byte ByteSum { get; set; }

    //    public MedtronicMessage2()
    //    {
    //    }

    //    public MedtronicMessage2(byte sessionNumber, AstmCommandType command) : this(sessionNumber, (byte)command)
    //    {
    //    }
    //    public MedtronicMessage2(byte sessionNumber, byte commandType) : base(new byte[] { 0x51, 0x03 })
    //    {

    //        this.SerialNumber = "000000";
    //        this.Unknown1 = Enumerable.Repeat<byte>(0x00, 10).ToArray();
    //        this.CommandType = commandType;
    //        this.SessionNumber = sessionNumber;
    //        this.Unknown2 = Enumerable.Repeat<byte>(0x00, 8).ToArray();

    //        this.Length = 0x00;
    //        this.Unknown3 = Enumerable.Repeat<byte>(0x00, 3).ToArray();
    //        this.ByteSum = 0x00;
    //    }


    //    public override string ToString()
    //    {
    //        return base.ToString() + " - " +this.CommandTypeName; 
    //    }
    //}

}
