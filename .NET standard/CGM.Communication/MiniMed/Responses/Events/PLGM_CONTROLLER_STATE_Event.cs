using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    //1317,08-06-17,00:08:11,08-06-17 00:08:11,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,PLGMControllerState,
    //"BEGIN_INTERNAL_STATE_AMOUNT=6, BEGIN_DELIVERY_STATE=true, BEGIN_REFERENCE_TIMER_STATE=false, BEGIN_REFRACTORY_ACTIVE_STATE=false, 
    //END_INTERNAL_STATE_AMOUNT=6, END_DELIVERY_STATE=true, END_REFERENCE_TIMER_STATE=false, END_REFRACTORY_ACTIVE_STATE=false, 
    //PREDICTED_SENSOR_GLUCOSE_AMOUNT=511, LAST_SG_AMOUNT=771, LOW_SG_ALERT_AMOUNT=54,048, REFRACTORY_TIMER=0, 
    //PLGM_INPUT_PATTERN_DATUM_ID=21151857573, PLGM_FLAG_PATTERN_DATUM_ID=21151857574",21151857575,59891650,3136,MiniMed 640G - 1511/1711

    public class PLGM_CONTROLLER_STATE_Event : BaseEvent
    {
        [BinaryElement(2,Length =2)]
        public Int16 PREDICTED_SENSOR_GLUCOSE_AMOUNT { get; set; }
        [BinaryElement(4, Length = 2)]
        public Int16 LAST_SG_AMOUNT { get; set; }

    }
}
