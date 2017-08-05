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

        public double BG
        {
            get
            {
                if (BgUnitName==BgUnitEnum.MG_DL)
                {
                    return (double)BG_RAW;
                }
                else
                {
                    return (double)BG_RAW / 10;
                }
                
            }
        }


        public override string ToString()
        {
            return BG.ToString();
        }
    }
}
