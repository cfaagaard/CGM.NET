using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{

    [BinaryType(IsLittleEndian = false)]
    public class EndHistoryTransmissionResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte Unknown { get; set; }

        [BinaryElement(1, Length = 4)]
        public byte[] FromRtc  { get; set; }

        [BinaryElement(5,Length =4)]
        public byte[] ToRtc { get; set; }

        public DateTime? FromDateTime { get; set; }

        public DateTime? ToDateTime { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.FromDateTime = DateTimeExtension.GetDateTime(FromRtc, settings.PumpTime.OffSet);
            //255*4 is now, I think..... need to handle this....
            this.ToDateTime = DateTimeExtension.GetDateTime(ToRtc, settings.PumpTime.OffSet);
        }

        public override string ToString()
        {
            if (FromDateTime.HasValue)
            {
                return FromDateTime.Value.ToString();
            }
            return "(NoDate)";
        }
    }
}
