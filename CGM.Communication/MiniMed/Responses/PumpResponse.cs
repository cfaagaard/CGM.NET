using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Requests;
using System;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    public class PumpResponse : MedtronicMessage3
    {
        [BinaryElement(2)]
        public byte Unknown1 { get; set; }

        [BinaryElement(3)]
        public byte AstmPayloadType { get; set; }

       // public AstmPayloadType AstmPayloadTypeName { get { return (AstmPayloadType)this.AstmPayloadTypeName; } }


    }
}
