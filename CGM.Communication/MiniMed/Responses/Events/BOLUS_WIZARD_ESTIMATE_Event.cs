using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Responses.Events
{

    //4929,10-06-17,21:08:28,10-06-17 21:08:28,,,,,,,,,,,,,,,"3,42","6,0","6,0",10,"2,5",0,"14,6","3,42",0,"0,0",,,,,,BolusWizardBolusEstimate,
    //"BG_INPUT=263,034, BG_UNITS=mmol l, CARB_INPUT=0, CARB_UNITS=grams, CARB_RATIO=10, INSULIN_SENSITIVITY=45,04, BG_TARGET_LOW=108,096, 
    //BG_TARGET_HIGH=108,096, BOLUS_ESTIMATE=3,425, CORRECTION_ESTIMATE=3,425, FOOD_ESTIMATE=0, UNABSORBED_INSULIN_TOTAL=0,
    //ACTION_REQUESTOR=dynamic, DYNAMIC_ACTION_REQUESTOR=0, IS_CLOSED_LOOP_ACTIVE=false",21175634396,59895147,2823,MiniMed 640G - 1511/1711
    [Serializable]
    public class BOLUS_WIZARD_ESTIMATE_Event : BaseEvent
    {
        [BinaryElement(0, Length = 1)]
        public byte BG_UNITS { get; set; }

        public BgUnitEnum BgUnitName { get { return (BgUnitEnum)BG_UNITS; } }

        [BinaryElement(1, Length = 1)]
        public byte CARB_UNITS { get; set; }

        public CarbUnitEnum CarbUnit { get { return (CarbUnitEnum)CARB_UNITS; } }

        [BinaryElement(2)]
        //[BinaryPropertyValueTransfer(ParentPropertyName = nameof(BG_UNITS), ChildPropertyName = nameof(BgDataType.BGUnits))]
        public Int16 BgInputRaw { get; set; }


        public double BgInput
        {
            get
            {
                if (this.BgUnitName == BgUnitEnum.MG_DL)
                {
                    return (double)BgInputRaw;
                }
                else
                {
                    return ((double)BgInputRaw / 10);
                }
            }
        }

        [BinaryElement(4)]
        public Int16 CarbInputaw { get; set; }


        public double CarbInput
        {
            get
            {
                if (this.CarbUnit == CarbUnitEnum.GRAMS)
                {
                    return (double)CarbInputaw;
                }
                else
                {
                    return ((double)CarbInputaw / 10);
                }
            }
        }

        [BinaryElement(6)] //  0x11
        public Int16 INSULIN_SENSITIVITY_Raw { get; set; }


        public double INSULIN_SENSITIVITY
        {
            get
            {
                if (this.BgUnitName == BgUnitEnum.MG_DL)
                {
                    return (double)INSULIN_SENSITIVITY_Raw;
                }
                else
                {
                    return ((double)INSULIN_SENSITIVITY_Raw / 10);
                }
            }
        }

        [BinaryElement(8)] //0x13
        public Int32 CarbRatioRaw { get; set; }

        public double CarbRatio
        {
            get
            {
                if (this.CarbUnit == CarbUnitEnum.GRAMS)
                {
                    return (double)CarbRatioRaw / 10;
                }
                else
                {
                    return CarbRatioRaw / 10000;
                }
            }
        }

        [BinaryElement(12)] //0x17
        public SettingType BG_TARGET_LOW { get; set; }


        [BinaryElement(14, Length = 2)] //19
        public SettingType BG_TARGET_HIGH { get; set; }

        //public int BOLUS_ESTIMATE { get; set; }
        [BinaryElement(16, Length = 1)]
        protected byte CORRECTION_ESTIMATE1 { get; set; }
        [BinaryElement(17, Length = 1)]
        protected byte CORRECTION_ESTIMATE2 { get; set; }
        [BinaryElement(18, Length = 1)]
        protected byte CORRECTION_ESTIMATE3 { get; set; }
        [BinaryElement(19, Length = 1)]
        protected byte CORRECTION_ESTIMATE4 { get; set; }

        public int CORRECTION_ESTIMATE
        {
            get
            {
                return
((CORRECTION_ESTIMATE1 << 8) | (this.CORRECTION_ESTIMATE2 << 8) | (CORRECTION_ESTIMATE3 << 8) | CORRECTION_ESTIMATE4) / 10000;
            }
        }

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

        [BinaryElement(37, Length = 1)]
        public byte EstimateModifiedByUserRaw { get; set; }

        public bool EstimateModifiedByUser { get { return (EstimateModifiedByUserRaw & 1) == 1; } }

        [BinaryElement(38, Length = 4)]
        public InsulinDataType FinalEstimate { get; set; }

        public override void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            base.OnDeserialization(bytes, settings);
        }

    }
}
