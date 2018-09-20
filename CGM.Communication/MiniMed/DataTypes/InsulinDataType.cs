using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{

    public class InsulinDataType : BaseDataType
    {
        [BinaryElement(0, Length = 4)]
        public Int32 InsulinRaw { get; set; }
        private double _INSULIN;
        public double Insulin { get { _INSULIN=(double)InsulinRaw / 10000;
                return _INSULIN;
            }
            set { _INSULIN = value; }
        }

        public override string ToString()
        {
            return Insulin.ToString();
        }
    }
}
