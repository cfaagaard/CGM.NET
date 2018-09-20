using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum ResumeReasonEnum
    {
        USER_SELECTS_RESUME = 1,
        USER_CLEARS_ALARM = 2,
        LGM_MANUAL_RESUME = 3,
        LGM_AUTO_RESUME_MAX_SUSP = 4, 
        LGM_AUTO_RESUME_PSG_SG = 5, 
        LGM_MANUAL_RESUME_VIA_DISABLE = 6
    }
}
