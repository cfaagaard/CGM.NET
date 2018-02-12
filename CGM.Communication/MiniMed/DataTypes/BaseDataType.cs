using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.DataTypes
{


    [BinaryType(IsLittleEndian = false)]
    public class BaseDataType : IBinaryType, IBinaryDeserializationSetting
    {
        public virtual void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
           
        }
    }

}
