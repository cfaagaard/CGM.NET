using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Interfaces
{
    public interface IStateRepository
    {
   
        void GetOrSetSessionAndSettings(SerializerSession session);
        void AddUpdateSessionToDevice(SerializerSession session);
    }
}
