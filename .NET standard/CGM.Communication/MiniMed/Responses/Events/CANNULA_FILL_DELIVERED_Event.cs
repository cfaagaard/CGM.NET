using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    //4651,10-06-17,16:01:28,10-06-17 16:01:28,,,,,,,,,,,Slangefyldning,"9,04",,,,,,,,,,,,,,,,,,Prime,"AMOUNT=9,04, PRIME_TYPE=tubing_fill, 
    //ACTION_REQUESTOR=dynamic, AMOUNT_AFTER_PRIME=148,975, DYNAMIC_ACTION_REQUESTOR=0, 
    //SYSTIME_RTC=2216012590, SYSTIME_OFFSET=-1665586902, IS_CLOSED_LOOP_ACTIVE=false",21175634181,59895147,2608,MiniMed 640G - 1511/1711
    public class CANNULA_FILL_DELIVERED_Event : BaseEvent
    {
        [BinaryElement(0)]
        public byte PRIME_TYPE { get; set; }
        public PrimeTypeEnum PRIME_TYPE_NAME { get { return (PrimeTypeEnum)this.PRIME_TYPE; } }
        [BinaryElement(1)]
        public InsulinDataType AMOUNT { get; set; }
        [BinaryElement(5)]
        public InsulinDataType AMOUNT_AFTER_PRIME { get; set; }
    }
}
