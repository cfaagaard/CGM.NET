using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{
    public class BgDataType : BaseDataType
    {
      
        public byte BGUnits { get; set; } = 0;

        public BgUnitEnum BgUnitName { get { return (BgUnitEnum)BGUnits; } }

        [BinaryElement(0, Length = 2)]
        public Int16 BG_RAW { get; set; }
        private double _BG;
        public double BG
        {
            get
            {
                if (BgUnitName==BgUnitEnum.MG_DL)
                {
                    _BG=(double)BG_RAW;
                }
                else
                {
                   //_BG=(double)BG_RAW / 10;
                    //should it not be
                    _BG= Math.Round(((double)this.BG_RAW / 18.01559), 1);
                }
                return _BG;
            }
            set { _BG = value; }
        }



        public BgDataType()
        {

        }

        public BgDataType(short bgRaw,BgUnitEnum bgUnit)
        {
            this.BG_RAW = bgRaw;
            this.BGUnits = (byte)bgUnit;
        }

        public override string ToString()
        {
            return BG.ToString();
        }
    }
}
