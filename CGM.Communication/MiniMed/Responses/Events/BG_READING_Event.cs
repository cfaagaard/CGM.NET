using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class BG_READING_Event : BaseEvent
    {

        [BinaryElement(0)] //11
        public byte BgUnitsRaw { get; set; }

        public BgUnitEnum BgUnits
        {
            get
            {
                if ((this.BgUnitsRaw== 0))
                {
                    return BgUnitEnum.MG_DL;
                }
                else
                {
                    return BgUnitEnum.MMOL_L;
                }
               // return (BgUnitEnum)this.BgUnitsRaw;
            }
        }

        [BinaryElement(1)] //12
        public short BgValueInMg { get; set; }

        public double BgValueInMmol { get { return Math.Round(((double)this.BgValueInMg / 18.01559), 1); } }

        [BinaryElement(3, Length = 1)] //14
        public byte BgSourceRaw { get; set; }


        public BgSourceEnum BgSource
        {
            get
            {

                return (BgSourceEnum)this.BgSourceRaw;
            }
        }

        [BinaryElement(4, Length = 10)]
        public byte[] RFIDRaw { get; set; }

        public string RFID { get { return Encoding.ASCII.GetString(RFIDRaw.Reverse().ToArray()); } }

        public string SerialNumSmall { get { return Regex.Replace(this.RFID, "[^0-9.]", ""); } }

        public bool CalibrationFlag { get { return (this.BgUnitsRaw & 2) == 2; } }

        public override string ToString()
        {
            return $"{this.BgSource} - {this.BgUnits} ({this.BgValueInMg}/{this.BgValueInMmol})";
        }
    }
}
