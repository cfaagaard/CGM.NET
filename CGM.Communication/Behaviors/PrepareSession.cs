using CGM.Communication.Common.Serialize;
using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Behaviors
{
    public class PrepareSessionBehavior : ISessionBehavior
    {
        public Task ExecuteTask(SerializerSession session, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }
    }
}
