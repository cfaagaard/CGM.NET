using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    [BinaryType()]
    public class LinkKeyResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte[] Key { get; set; }
        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.SessionCommunicationParameters.LinkKey = Key;
        }
    }
}
