using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{

    public class InsulinDataType : BaseDataType
    {
        [BinaryElement(0, Length = 4)]
        public Int32 INSULIN_RAW { get; set; }
        public double INSULIN { get { return (double)INSULIN_RAW / 10000; } }

        public override string ToString()
        {
            return INSULIN.ToString();
        }
    }
}
