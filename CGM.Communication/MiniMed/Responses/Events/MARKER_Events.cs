using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class Base_MARKER_Event : BaseEvent
    {
        [BinaryElement(0)]
        public DateTimeDataType StartDatetime { get; set; }

    }
    [Serializable]
    public class EXERCISE_MARKER_Event: Base_MARKER_Event
    {
        [BinaryElement(8)]
        public Int16 Duration { get; set; }

        public override string ToString()
        {
            return $"EXERCISE_MARKER ({Duration})";
        }
    }

    [Serializable]
    public class INJECTION_MARKER_Event : Base_MARKER_Event
    {
        [BinaryElement(8)]
        public InsulinDataType Insulin { get; set; }

        public override string ToString()
        {
            return $"INJECTION_MARKER ({Insulin.Insulin})";
        }
    }
    [Serializable]
    public class FOOD_MARKER_Event : Base_MARKER_Event
    {
        [BinaryElement(9)]
        public CarbDataType Carbs { get; set; }

        public override string ToString()
        {
            return $"FOOD_MARKER ({Carbs.CARB})";
        }
    }
    [Serializable]
    public class OTHER_MARKER_Event : Base_MARKER_Event
    {
        public override string ToString()
        {
            return $"OTHER_MARKER";
        }
    }
    
}
