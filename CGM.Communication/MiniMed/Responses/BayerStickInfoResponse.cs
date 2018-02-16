using CGM.Communication.Common.Serialize;
using System;
using System.Text;
using CGM.Communication.Extensions;
using System.Text.RegularExpressions;
using CGM.Communication.MiniMed.Requests;
using MongoDB.Bson.Serialization.Attributes;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType]
    public class BayerStickInfoResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public string Value { get; set; }
        [BsonId]
        public string SerialNumberFull { get; set; }

        public string RFID { get; set; }

        public string ModelNumber { get; set; }

        public string SerialNumber { get; set; }

        public byte[] HMACbyte { get; set; }

        public BayerStickInfoReader Reader { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {

            Reader = new BayerStickInfoReader(this.Value);

            this.ModelNumber = Reader.DeviceVersion.Name;
            this.RFID = Reader.DeviceVersion.RFID;
            this.SerialNumberFull = Reader.DeviceVersion.SerialNum;
            this.SerialNumber = Reader.DeviceVersion.SerialNumSmall;
            this.HMACbyte = Reader.HMACbyte;

            if (settings!=null)
            {
                settings.SessionDevice.Device = this;
            }
           
        }

        public override string ToString()
        {
            return string.Format("StickInfo: {0}", this.SerialNumberFull);
        }
    }
}
