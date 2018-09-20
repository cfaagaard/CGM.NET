using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Interfaces
{
    public interface ISessionBehavior
    {
        Task ExecuteTask(SerializerSession session, CancellationToken cancelToken);
    }

    public interface ISessionBehaviors
    {
        List<ISessionBehavior> SessionBehaviors { get; set; }
    }
}
