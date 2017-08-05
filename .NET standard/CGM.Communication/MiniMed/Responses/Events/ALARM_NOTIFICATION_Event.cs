using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    //"2337,08-06-17,19:48:17,08-06-17 19:48:17,,,,,,,,,,,,,,,,,,,,,,,,,Sensoradvarsel: Kalibreringspåmindelse (869),,,,,AlarmPumpNGP,
    //\"RAW_TYPE=869, RAW_MODULE=197, LINE_NUM=384, NOTIFY_MODE=vibration, DIAGNOSTIC_INFO=1, INSULIN_DELIVERY_FLAG=0, 
    //ALARM_HISTORY_SCREEN_FLAG=1, NOTIFICATION_SCREEN_FLAG=1, DYNAMIC_ACTION_REQUESTOR=0, SYSTIME_RTC=2215853399, SYSTIME_OFFSET=-1665586902, 
    //IS_CLOSED_LOOP_ACTIVE=false, EXTRA_DATA=\u00140\u0084\",21151858359,59891650,3920,MiniMed 640G - 1511/1711"
   public class ALARM_NOTIFICATION_Event: ALARM_CLEARED_Event
    {

    }


}
