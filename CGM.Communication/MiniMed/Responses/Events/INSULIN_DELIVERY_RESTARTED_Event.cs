using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    public class INSULIN_DELIVERY_RESTARTED_Event : BaseEvent
    {

        [BinaryElement(0)]
        public byte ResumeReasonRaw { get; set; }

        public ResumeReasonEnum ResumeReason { get { return (ResumeReasonEnum)ResumeReasonRaw; } }

        public override string ToString()
        {
            return this.ResumeReason.ToString();
        }
    }
}
