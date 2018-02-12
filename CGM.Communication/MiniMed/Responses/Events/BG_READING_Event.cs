using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGM.Communication.MiniMed.Responses.Events
{
    public class BG_READING_Event : BaseEvent
    {

        [BinaryElement(0)] //11
        public byte BgUnitsRaw { get; set; }

        public BgUnitEnum BgUnits { get { return (BgUnitEnum)this.BgUnitsRaw; } }

        [BinaryElement(1)] //12
        public short BgValueInMg { get; set; }

        public double BgValueInMmol { get { return Math.Round(((double)this.BgValueInMg / 18.01559), 1); } }

        [BinaryElement(3, Length =1)] //14
        public byte  BgSourceRaw{ get; set; }


        public BgSourceEnum BgSource { get { return (BgSourceEnum)this.BgSourceRaw; } }

        public override string ToString()
        {
            return $"{this.Timestamp.Value} - {this.BgSource} - {this.BgUnits} ({this.BgValueInMg}/{this.BgValueInMmol})";
        }
    }
}
