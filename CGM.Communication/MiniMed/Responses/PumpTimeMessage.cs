using CGM.Communication.Common.Serialize;
using System;
using CGM.Communication.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace CGM.Communication.MiniMed.Responses
{
    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class PumpTimeMessage : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte TimeSet { get; set; }

        [BinaryElement(1)]
        public byte[] Rtc { get; set; }

        [BinaryElement(5)]
        public byte[] OffSet { get; set; }

        [BinaryElement(9)]
        public byte[] Unknown2 { get; set; }

        [BinaryElement(11)]
        public byte[] Crc16citt { get; set; }

        public DateTime? PumpDateTime
        {
            get
            {
                if (this.Rtc!=null && this.OffSet!=null)
                {
                    return DateTimeExtension.GetDateTime(this.Rtc, this.OffSet);
                }
                return null;
               
            }
        }
        public byte[] AllBytes { get; set; }
        public DateTime? GetDateTime(byte[] Rtc)
        {
            if (Rtc != null && this.OffSet != null)
            {
                return DateTimeExtension.GetDateTime(Rtc, this.OffSet);
            }
            return null;
        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.PumpTime = this;
            AllBytes = bytes;
        }

        public override string ToString()
        {
            return PumpDateTime?.ToString();
        }
    }
}
