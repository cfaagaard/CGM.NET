using CGM.Communication.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Interfaces
{
    public interface ICgmTask
    {
        void Start();
        void Stop();

        TaskStatusEnum Status { get; set; }
        event EventHandler StatusChanged;

    }
}
