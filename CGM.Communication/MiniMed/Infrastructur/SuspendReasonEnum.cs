using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum SuspendReasonEnum
    {
        ALARM_SUSPEND= 1, 
        USER_SUSPEND= 2,
        AUTO_SUSPEND= 3,
        LOWSG_SUSPEND= 4,
        SET_CHANGE_SUSPEND= 5,
        PLGM_PREDICTED_LOW_SG= 10,
    }
}
