using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;

namespace CGM.Communication.MiniMed.Responses.Events
{

    //4929,10-06-17,21:08:28,10-06-17 21:08:28,,,,,,,,,,,,,,,"3,42","6,0","6,0",10,"2,5",0,"14,6","3,42",0,"0,0",,,,,,BolusWizardBolusEstimate,
    //"BG_INPUT=263,034, BG_UNITS=mmol l, CARB_INPUT=0, CARB_UNITS=grams, CARB_RATIO=10, INSULIN_SENSITIVITY=45,04, BG_TARGET_LOW=108,096, 
    //BG_TARGET_HIGH=108,096, BOLUS_ESTIMATE=3,425, CORRECTION_ESTIMATE=3,425, FOOD_ESTIMATE=0, UNABSORBED_INSULIN_TOTAL=0,
    //ACTION_REQUESTOR=dynamic, DYNAMIC_ACTION_REQUESTOR=0, IS_CLOSED_LOOP_ACTIVE=false",21175634396,59895147,2823,MiniMed 640G - 1511/1711

    public class BOLUS_WIZARD_ESTIMATE_Event : BaseEvent
    {
        [BinaryElement(0, Length = 1)]
        public byte BG_UNITS { get; set; }

        [BinaryElement(1, Length = 1)]
        public byte CARB_UNITS { get; set; }

        [BinaryElement(2)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(BG_UNITS), ChildPropertyName = nameof(BgDataType.BGUnits))]
        public BgDataType BG_INPUT { get; set; }

        [BinaryElement(4)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(CARB_UNITS), ChildPropertyName = nameof(CarbDataType.CarbUnits))]
        public CarbDataType CARB_INPUT { get; set; }


        [BinaryElement(8)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(CARB_UNITS), ChildPropertyName = nameof(CarbDataType.CarbUnits))]
        public CarbRatioDataType CARB_RATIO { get; set; }


        [BinaryElement(10)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(BG_UNITS), ChildPropertyName = nameof(BgDataType.BGUnits))]
        public BgDataType INSULIN_SENSITIVITY { get; set; }

        [BinaryElement(12)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(BG_UNITS), ChildPropertyName = nameof(BgDataType.BGUnits))]
        public BgDataType BG_TARGET_LOW { get; set; }


        [BinaryElement(14, Length = 2)]
        [BinaryPropertyValueTransfer(ParentPropertyName = nameof(BG_UNITS), ChildPropertyName = nameof(BgDataType.BGUnits))]
        public BgDataType BG_TARGET_HIGH { get; set; }


        //public int BOLUS_ESTIMATE { get; set; }

        //public int CORRECTION_ESTIMATE { get; set; }
        //public int UNABSORBED_INSULIN_TOTAL { get; set; }


        [BinaryElement(20, Length = 4)]
        public InsulinDataType FOOD_ESTIMATE { get; set; }

        [BinaryElement(24, Length = 4)]
        public InsulinDataType IOB { get; set; }
        [BinaryElement(28, Length = 4)]
        public InsulinDataType IobAdjustment { get; set; }
        [BinaryElement(32, Length = 4)]
        public InsulinDataType BolusWizardEstimate { get; set; }


        [BinaryElement(36, Length = 1)]
        public byte bolusStepSize { get; set; }

        [BinaryElement(37, Length = 4)]
        public byte EstimateModifiedByUserRaw { get; set; }

        public bool EstimateModifiedByUser { get { return (EstimateModifiedByUserRaw & 1) == 1;  } }
        
        [BinaryElement(38, Length = 4)]
        public InsulinDataType FinalEstimate { get; set; }


        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
        }

    }
}
