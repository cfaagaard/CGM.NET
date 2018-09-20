using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common
{
    public class CgmSessionBehaviors : ISessionBehaviors
    {
        public List<ISessionBehavior> SessionBehaviors { get; set; } = new List<ISessionBehavior>();
    }
}
