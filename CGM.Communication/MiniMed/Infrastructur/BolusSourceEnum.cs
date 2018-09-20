using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum BolusSourceEnum
    {
        MANUAL= 0,
        BOLUS_WIZARD= 1,
        EASY_BOLUS= 2,
        PRESET_BOLUS= 4,
        CLOSED_LOOP_MICRO_BOLUS= 5,
        CLOSED_LOOP_BG_CORRECTION= 6,
        CLOSED_LOOP_FOOD_BOLUS= 7,
        CLOSED_LOOP_BG_CORRECTION_AND_FOOD_BOLUS=8
    }
}
