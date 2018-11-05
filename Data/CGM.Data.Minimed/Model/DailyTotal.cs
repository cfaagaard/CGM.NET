using CGM.Data.Minimed.Model.Owned;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class DailyTotal
    {

        public int DailyTotalId { get; set; }
        public int PumpEventId { get; set; }
        public PumpEvent PumpEvent { get; set; }

        public DateTimeDataType Date { get; set; }

        
        public Int16 Duration { get; set; }
    
        public byte METER_BG_COUNT { get; set; }


        public BgDataType METER_BG_AVERAGE { get; set; }
      
        public BgDataType LOW_METER_BG { get; set; }

      
        public BgDataType HIGH_METER_BG { get; set; }

     
        public byte MANUALLY_ENTERED_BG_COUNT { get; set; }

     
        public BgDataType MANUALLY_ENTERED_BG_AVERAGE { get; set; }

        public BgDataType LOW_MANUALLY_ENTERED_BG { get; set; }

        public BgDataType HIGH_MANUALLY_ENTERED_BG { get; set; }

        public BgDataType BG_AVERAGE { get; set; }

      
        public InsulinDataType TOTAL_INSULIN { get; set; }

       
        public InsulinDataType BASAL_INSULIN { get; set; }


     
        public byte BASAL_PERCENT { get; set; }

    
        public InsulinDataType BOLUS_INSULIN { get; set; }


      
        public byte BOLUS_PERCENT { get; set; }

     
        public byte CARB_UNITS { get; set; }

       
        public Int16 TOTAL_FOOD_INPUT { get; set; }


      
        public byte BOLUS_WIZARD_USAGE_COUNT { get; set; }

    
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS { get; set; }

       
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS { get; set; }

     
        public InsulinDataType TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION { get; set; }

     
        public InsulinDataType TOTAL_MANUAL_BOLUS_INSULIN { get; set; }

     
        public byte BOLUS_WIZARD_FOOD_ONLY_BOLUS_COUNT { get; set; }

      
        public byte BOLUS_WIZARD_CORRECTION_ONLY_BOLUS_COUNT { get; set; }


    
        public byte BOLUS_WIZARD_FOOD_AND_CORRECTION_BOLUS_COUNT { get; set; }

       
        public byte MANUAL_BOLUS_COUNT { get; set; }

   
        public Int16 SG_COUNT { get; set; }


   
        public SgDataType SG_AVERAGE { get; set; }

       
        public SgDataType SG_STDDEV { get; set; }

       
        public Int16 SG_DURATION_ABOVE_HIGH { get; set; }

      
        public byte PERCENT_ABOVE_HIGH { get; set; }

       
        public Int16 SG_DURATION_WITHIN_LIMIT { get; set; }
      
        public byte PERCENT_WITHIN_LIMIT { get; set; }
       
        public Int16 SG_DURATION_BELOW_LOW { get; set; }
       
        public byte PERCENT_BELOW_LOW { get; set; }
       
        public Int16 LGS_SUSPENSION_DURATION { get; set; }

       
        public Int16 HIGH_PREDICTIVE_ALERTS { get; set; }

        
        public Int16 LOW_PREDICTIVE_ALERTS { get; set; }

        
        public Int16 LOW_BG_ALERTS { get; set; }

   
        public Int16 HIGH_BG_ALERTS { get; set; }

 
        public Int16 RISING_RATE_ALERTS { get; set; }

      
        public Int16 FALLING_RATE_ALERTS { get; set; }

        
        public Int16 LOW_GLUCOSE_SUSPEND_ALERTS { get; set; }

       
        public Int16 PREDICTIVE_LOW_GLUCOSE_SUSPEND_ALERTS { get; set; }
    }
}
