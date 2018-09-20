using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{
    public class SettingType : BaseDataType
    {

        [BinaryElement(0, Length = 2)]
        public Int16 RAW { get; set; }

        private double _units;
        public virtual double Units
        {
            get
            {
                return _units = (double)RAW / 10;
            }
        }

        public override string ToString()
        {
            return Units.ToString();
        }

    }
}
