using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Interfaces
{
    public interface ITask
    {
        void Start(IDevice device);
        void Stop();
    }
}
