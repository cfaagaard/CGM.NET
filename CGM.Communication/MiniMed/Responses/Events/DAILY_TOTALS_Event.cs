using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Responses.Events
{
    [Serializable]
    public class DAILY_TOTALS_Event : BaseEvent
    {
        [BinaryElement(0)]
        public DateTimeDataType Date { get; set; }

        [BinaryElement(8)]
        public Int16 Duration { get; set; }
        [BinaryElement(10)]
        public byte METER_BG_COUNT { get; set; }

        [BinaryElement(11)]
        public BgDataType METER_BG_AVERAGE { get; set; }
        [BinaryElement(13)]
        public BgDataType LOW_METER_BG { get; set; }

        [BinaryElement(15)]
        public BgDataType HIGH_METER_BG { get; set; }

        [BinaryElement(17)]
        public byte MANUALLY_ENTERED_BG_COUNT { get; set; }

        [BinaryElement(18)]
        public BgDataType MANUALLY_ENTERED_BG_AVERAGE { get; set; }

        [BinaryElement(20)]
        public BgDataType LOW_MANUALLY_ENTERED_BG { get; set; }

        [BinaryElement(22)]
        public BgDataType HIGH_MANUALLY_ENTERED_BG { get; set; }

        [BinaryElement(24)]
        public BgDataType BG_AVERAGE { get; set; }

        [BinaryElement(26)]
        public InsulinDataType TOTAL_INSULIN { get; set; }

        [BinaryElement(30)]
        public InsulinDataType BASAL_INSULIN { get; set; }


        [BinaryElement(34)]
        public byte BASAL_PERCENT { get; set; }

        [BinaryElement(35)]
        public InsulinDataType BOLUS_INSULIN { get; set; }


        [BinaryElement(39)]
        public byte BOLUS_PERCENT { get; set; }

        [BinaryElement(40)]
        public byte CARB_UNITS { get; set; }

        [BinaryElement(41)]
        public Int16 TOTAL_FOOD_INPUT { get; set; }


        [BinaryElement(43)]
        public byte BOLUS_WIZARD_USAGE_COUNT { get; set; }

        [BinaryElement(44)]
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS { get; set; }

        [BinaryElement(48)]
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS { get; set; }

        [BinaryElement(52)]
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION { get; set; }

        [BinaryElement(56)]
        public InsulinDataType TOTAL_MANUAL_BOLUS_INSULIN { get; set; }

        [BinaryElement(60)]
        public byte BOLUS_WIZARD_FOOD_ONLY_BOLUS_COUNT { get; set; }

        [BinaryElement(61)]
        public byte BOLUS_WIZARD_CORRECTION_ONLY_BOLUS_COUNT { get; set; }


        [BinaryElement(62)]
        public byte BOLUS_WIZARD_FOOD_AND_CORRECTION_BOLUS_COUNT { get; set; }

        [BinaryElement(63)]
        public byte MANUAL_BOLUS_COUNT { get; set; }

        [BinaryElement(64)]
        public Int16 SG_COUNT { get; set; }


        [BinaryElement(66)]
        public SgDataType SG_AVERAGE { get; set; }

        [BinaryElement(68)]
        public SgDataType SG_STDDEV { get; set; }

        [BinaryElement(70)]
        public Int16 SG_DURATION_ABOVE_HIGH { get; set; }

        [BinaryElement(72)]
        public byte PERCENT_ABOVE_HIGH { get; set; }

        [BinaryElement(73)]
        public Int16 SG_DURATION_WITHIN_LIMIT { get; set; }
        [BinaryElement(75)]
        public byte PERCENT_WITHIN_LIMIT { get; set; }
        [BinaryElement(76)]
        public Int16 SG_DURATION_BELOW_LOW { get; set; }
        [BinaryElement(78)]
        public byte PERCENT_BELOW_LOW { get; set; }
        [BinaryElement(79)]
        public Int16 LGS_SUSPENSION_DURATION { get; set; }

        [BinaryElement(81)]
        public Int16 HIGH_PREDICTIVE_ALERTS { get; set; }

        [BinaryElement(85)]
        public Int16 LOW_PREDICTIVE_ALERTS { get; set; }

        [BinaryElement(87)]
        public Int16 LOW_BG_ALERTS { get; set; }

        [BinaryElement(89)]
        public Int16 HIGH_BG_ALERTS { get; set; }

        [BinaryElement(91)]
        public Int16 RISING_RATE_ALERTS { get; set; }

        [BinaryElement(93)]
        public Int16 FALLING_RATE_ALERTS { get; set; }

        [BinaryElement(95)]
        public Int16 LOW_GLUCOSE_SUSPEND_ALERTS { get; set; }

        [BinaryElement(97)]
        public Int16 PREDICTIVE_LOW_GLUCOSE_SUSPEND_ALERTS { get; set; }
    }


    //source: pogman
    //     ?0 ?1 ?2 ?3 ?4 ?5 ?6 ?7 ?8 ?9 ?A ?B ?C ?D ?E ?F
    //0x00 CR CR CR CR CO CO CO CO CD CD BG BA BA BL BL BH
    //0x10 BH MG MA MA ML ML MH MH BV BV TD TD TD TD TB TB
    //0x20 TB TB PB TI TI TI TI PI WW WA WA WB WC WC WC WC
    //0x30 WD WD WD WD WE WE WE WE WF WF WF WF WG WH WI WJ
    //0x40 ST ST SA SA SD SD SX SX SH SY SY SI SZ SZ SL SK
    //0x50 SK AA AA AB AB AC AC AD AD AE AE AF AF AG AG AH
    //0x60 AH

    //CR = RTC
    //CO - OFFSET
    //CD = DURATION(mins)

    //BG = METER_BG_COUNT
    //BA = METER_BG_AVERAGE(mg / dL)
    //BL = LOW_METER_BG(mg/dL)
    //BH = HIGH_METER_BG(mg/dL)
    //MG = MANUALLY_ENTERED_BG_COUNT
    //MA = MANUALLY_ENTERED_BG_AVERAGE(mg / dL)
    //ML = LOW_MANUALLY_ENTERED_BG(mg/dL)
    //MH = HIGH_MANUALLY_ENTERED_BG(mg/dL)
    //BV = BG_AVERAGE(mg/dL)

    //TD = TOTAL_INSULIN(div 10000)
    //TB = BASAL_INSULIN(div 10000)
    //PB = BASAL_PERCENT
    //TI = BOLUS_INSULIN(div 10000)
    //PI = BOLUS_PERCENT

    //WW = CARB_UNITS
    //WA = TOTAL_FOOD_INPUT
    //WB = BOLUS_WIZARD_USAGE_COUNT
    //WC = TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS(div 10000)
    //WD = TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS(div 10000)
    //WE = TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION(div 10000)
    //WF = TOTAL_MANUAL_BOLUS_INSULIN(div 10000)
    //WG = BOLUS_WIZARD_FOOD_ONLY_BOLUS_COUNT
    //WH = BOLUS_WIZARD_CORRECTION_ONLY_BOLUS_COUNT
    //WI = BOLUS_WIZARD_FOOD_AND_CORRECTION_BOLUS_COUNT
    //WJ = MANUAL_BOLUS_COUNT

    //ST = SG_COUNT
    //SA = SG_AVERAGE(mg / dL)
    //SD = SG_STDDEV(mg/dL)
    //SX = SG_DURATION_ABOVE_HIGH(mins)
    //SH = PERCENT_ABOVE_HIGH
    //SX = SG_DURATION_WITHIN_LIMIT(mins)
    //SI = PERCENT_WITHIN_LIMIT
    //SZ = SG_DURATION_BELOW_LOW(mins)
    //SL = PERCENT_BELOW_LOW
    //SK = LGS_SUSPENSION_DURATION(mins)

    //AA = HIGH_PREDICTIVE_ALERTS
    //AB = LOW_PREDICTIVE_ALERTS
    //AC = LOW_BG_ALERTS
    //AD = HIGH_BG_ALERTS
    //AE = RISING_RATE_ALERTS
    //AF = FALLING_RATE_ALERTS
    //AG = LOW_GLUCOSE_SUSPEND_ALERTS
    //AH = PREDICTIVE_LOW_GLUCOSE_SUSPEND_ALERTS
}
