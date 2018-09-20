using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{
    public class SgDataType : BaseDataType
    {
      
        public byte BGUnits { get; set; } = 0;

        public BgUnitEnum BgUnitName { get { return (BgUnitEnum)BGUnits; } }

        [BinaryElement(0, Length = 2)]
        public Int16 SG_RAW { get; set; }
        private double _SG;
        public double SG
        {
            get
            {
                if (BgUnitName==BgUnitEnum.MG_DL)
                {
                    _SG=(double)SG_RAW;
                }
                else
                {
                    _SG= Math.Round(((double)this.SG_RAW / 18.01559), 1);
                }
                return _SG;
            }
            set { _SG = value; }
        }



        public SgDataType()
        {

        }

        public SgDataType(short bgRaw,BgUnitEnum bgUnit)
        {
            this.SG_RAW = bgRaw;
            this.BGUnits = (byte)bgUnit;
        }

        public override string ToString()
        {
            return SG.ToString();
        }
    }
}
