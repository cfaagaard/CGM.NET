
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class DUAL_BOLUS_DELIVERED_Event : BOLUS_Event
    {
        [BinaryElement(3)]
        public InsulinDataType NormalProgrammedAmount { get; set; }

        [BinaryElement(7)]
        public InsulinDataType SquareProgrammedAmount  { get; set; }

        [BinaryElement(11)]
        public InsulinDataType DeliveredAmount { get; set; }

        [BinaryElement(15)]
        public byte BolusPart { get; set; }

        [BinaryElement(16)]
        public UInt16 ProgrammedDuration { get; set; }

        [BinaryElement(18)]
        public UInt16 DeliveredDuration { get; set; }

        [BinaryElement(20)]
        public InsulinDataType InsulinOnBoard { get; set; }
    }


}
