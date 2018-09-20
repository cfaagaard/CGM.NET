using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{
    public class CarbDataType : BaseDataType
    {

        public byte CarbUnits { get; set; }
        public CarbUnitEnum CarbUnitName { get { return (CarbUnitEnum)CarbUnits; } }

        [BinaryElement(0, Length = 2)]
        public Int16 CARB_RAW { get; set; }
        private double _CARB;
        public virtual double CARB
        {
            get
            {
                if (CarbUnitName == CarbUnitEnum.GRAMS)
                {
                    _CARB=(double)CARB_RAW;
                }
                else
                {
                    _CARB=(double)CARB_RAW / 10;
                }
                return _CARB;
            }
            set { _CARB = value; }
        }

        public override string ToString()
        {
            return CARB.ToString();
        }
    }

    public class CarbRatioDataType: CarbDataType
    {
        public override  double CARB
        {
            get
            {
                if (CarbUnitName == CarbUnitEnum.GRAMS)
                {
                    return (double)CARB_RAW / 10;
                }
                else
                {
                    return (double)CARB_RAW / 1000;
                }

            }
        }
    }
}
