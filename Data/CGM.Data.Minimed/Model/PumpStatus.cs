using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class PumpStatus
    {
        public int PumpStatusId { get; set; }

        public int PumpId { get; set; }

        public virtual Pump Pump { get; set; }

        public DateTime PumpStatusDateTime { get; set; } 

        public byte StatusFlag { get; set; }

        public int NowBolusingAmountDelivered { get; set; }

        public int Unknown1 { get; set; }

        public Int16 NowBolusingMinutesRemaining { get; set; }

        public Int16 NowBolusingReference { get; set; }

        public int LastBolusAmount { get; set; }

        public int LastBolusTime { get; set; }
       
        public DateTime? LastBolusDateTime { get; set; }

        public Int16 LastBolusReference { get; set; }
    
        public byte ActiveBasalPattern { get; set; }

        public int NormalBasalRaw { get; set; }
      
        public int TempBasal { get; set; }
    
        public byte TempBasalPercentage { get; set; }

        public Int16 TempBasalMinutesRemaining { get; set; }
    
        public int BasalUnitsDeliveredTodayRaw { get; set; }
  
        public byte BatteryPercentage { get; set; }

        public int ReservoirAmountRaw { get; set; }

        public byte InsulinHours { get; set; }

        public byte InsulinMinutes { get; set; }
 
        public Int32 ActiveInsulinRaw { get; set; }

        public double ActiveInsulin { get; set; }

        public Int16 SgvRaw { get; set; }

        public int Sgv { get; set; }
 
        public DateTime SgvDateTime { get; set; }

        public byte LowSuspendActive { get; set; }

        public byte CgmTrend { get; set; }
 
        public byte SensorStatusFlag { get; set; }

        public byte Unknown3 { get; set; }

        public Int16 SensorCalibrationMinutesRemaining { get; set; }
 
        public DateTime? SensorCalibrationDateTime { get; set; }
  
        public byte SensorBatteryRaw { get; set; }
   
        public int SensorBattery { get; set; }
  
        public Int16 SensorRateOfChangeRaw { get; set; }

        public byte BolusWizardRecent { get; set; }
      
        public Int16 BolusWizardBGLRaw { get; set; }

        public int BolusWizardBGL { get; set; }

        public Int16 Alert { get; set; }

        public DateTime? AlertDateTime { get; set; }

        public byte[] Unknown6 { get; set; }
   
        public double SgvMmol { get; set; }
   
        public double NormalBasal { get; set; }

        public double BolusEstimate { get; set; }
    
        public double BasalUnitsDeliveredToday { get; set; }

        public double ReservoirAmount { get; set; }

        public string AlertName { get; set; }

        //PS=PumpStatus
        public bool PS_Suspended { get; set; }
        public bool PS_BolusingNormal { get; set; }
        public bool PS_BolusingSquare { get; set; }
        public bool PS_BolusingDual { get; set; }
        public bool PS_DeliveringInsulin { get; set; }
        public bool PS_CgmActive { get; set; }
        public bool PS_TempBasalActive { get; set; }

        //SS=SensorStatus
        public bool SS_Calibrating { get; set; }
        public bool SS_CalibrationComplete { get; set; }
        public bool SS_Exception { get; set; }

 
        public string BytesAsString { get; set; }
 
        public override string ToString()
        {
            return string.Format("{0} ({1}/{2})", this.SgvDateTime.ToString(), this.Sgv, this.SgvMmol);
        }
    }
}
