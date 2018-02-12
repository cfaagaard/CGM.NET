using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed
{
    public enum RequestEnum:int
    {

        GetDeviceInformation=0,
        GetPumpStatus=1,
        GetBasalPatterns=2,
        GetCarbRatio=3,
        GetHistory=4


    }
}
