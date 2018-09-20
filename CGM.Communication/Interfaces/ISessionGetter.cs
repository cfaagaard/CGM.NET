using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Interfaces
{
    public interface ISessionFactory
    {
       Task<SerializerSession> GetPumpSessionAsync(SerializerSession session, CancellationToken token);
    }
}
