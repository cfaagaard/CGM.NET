using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Interfaces
{
    public interface IReportPattern
    {
        bool Evaluate(Byte[] bytes);

    }
}
