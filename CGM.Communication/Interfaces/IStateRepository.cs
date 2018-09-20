using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Responses.Events;

namespace CGM.Communication.Interfaces
{
    public interface IStateRepository
    {  
        void GetOrSetSessionAndSettings(SerializerSession session);
        void AddUpdateSessionToDevice(SerializerSession session);

        void SaveSession(SerializerSession session);
        void SaveConfiguration<T>(T configuration);
        void AddKeys(List<string> keys);

        List<BasePumpEvent> GetHistoryWithNoStatus(SerializerSession session);
        List<BasePumpEvent> GetHistory(List<int> eventFilter, SerializerSession session);
    }
}
