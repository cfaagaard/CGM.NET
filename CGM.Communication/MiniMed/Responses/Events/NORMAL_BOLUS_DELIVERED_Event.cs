using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using System;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class NORMAL_BOLUS_DELIVERED_Event : BOLUS_Event
    {
        [BinaryElement(3)]
        public InsulinDataType ProgrammedAmount { get; set; }

        [BinaryElement(7)]
        public InsulinDataType DeliveredAmount { get; set; }

      
        [BinaryElement(11)]
        public InsulinDataType InsulinOnBoard { get; set; }
    }


}
