using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGM.Communication.MiniMed.Responses.Events
{
    public class ALARM_CLEARED_Event : BaseEvent
    {
        [BinaryElement(0, Length = 2)]
        public Int16 AlarmType { get; set; }

        public Alerts AlarmTypeName { get { return (Alerts)this.AlarmType; } }

    }
}
