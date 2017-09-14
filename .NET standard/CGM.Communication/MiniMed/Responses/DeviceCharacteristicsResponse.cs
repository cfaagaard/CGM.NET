using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class DeviceCharacteristicsResponse : IBinaryType, IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public byte[] SerialNumberRaw { get; set; }

        public string SerialNumber { get { return Encoding.ASCII.GetString(SerialNumberRaw.Reverse().ToArray()); } }

        [BinaryElement(10,Length =8)]
        public byte[] Mac { get; set; }

        //[BinaryElement(18)]
        //public byte ComMajorNumber { get; set; }

        //[BinaryElement(19)]
        //public byte ComMinorNumber { get; set; }

        //[BinaryElement(20)]
        //public string ComAlpha { get; set; }

        //[BinaryElement(21)]
        //public byte TelMajorNumber { get; set; }

        //[BinaryElement(22)]
        //public byte TelMinorNumber { get; set; }

        //[BinaryElement(23)]
        //public byte TelAlpha { get; set; }

        //[BinaryElement(23)]
        //public Int16 ModelMajorNumber { get; set; }

        //[BinaryElement(25)]
        //public Int16 ModelMinorNumber { get; set; }

        [BinaryElement(38)]
        public byte FirmwareMajorNumber { get; set; }

        [BinaryElement(39)]
        public byte FirmwareMinorNumber { get; set; }

        [BinaryElement(40)]
        public string FirmwareAlphaNumber { get; set; }

        public string Firmware { get { return $"{FirmwareMajorNumber}.{FirmwareMinorNumber}{FirmwareAlphaNumber}"; } }

        [BinaryElement(41)]
        public byte MotorMajorNumber { get; set; }

        [BinaryElement(42)]
        public byte MotorMinorNumber { get; set; }

        [BinaryElement(43,Length =1)]
        public string MotorAlphaNumber { get; set; }

        public string Motor { get { return $"{MotorMajorNumber}.{MotorMinorNumber}{MotorAlphaNumber}"; } }

        [BinaryElement(50,Length =1)]
        public byte BgUnitRaw { get; set; }

        public BgUnitEnum BgUnit { get { return (BgUnitEnum)BgUnitRaw; } }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.DeviceCharacteristics = this;
        }
    }
}
