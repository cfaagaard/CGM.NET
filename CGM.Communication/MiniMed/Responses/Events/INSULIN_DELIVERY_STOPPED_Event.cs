using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    class INSULIN_DELIVERY_STOPPED_Event : BaseEvent
    {

        [BinaryElement(0)]
        public byte SuspendReasonRaw { get; set; }

        public SuspendReasonEnum SuspendReason { get { return (SuspendReasonEnum)SuspendReasonRaw; } }

        public override string ToString()
        {
            return SuspendReason.ToString();
        }
    }
}
