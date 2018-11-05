using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGM.Communication.MiniMed.Responses
{
    public enum StatusFlag
    {
        No_sgv = 16
    }

    [Serializable]
    [BinaryType(IsLittleEndian = false)]
    public class PumpStatusMessage : IBinaryType, IBinaryDeserializationSetting
    {

    

        [BinaryElement(0)]
        public byte StatusFlag { get; set; }

        [BinaryElement(1)]
        public int NowBolusingAmountDelivered { get; set; }

        [BinaryElement(5)]
        public int Unknown1 { get; set; }

        [BinaryElement(9)]
        public Int16 NowBolusingMinutesRemaining { get; set; }

        [BinaryElement(11)]
        public Int16 NowBolusingReference { get; set; }

        [BinaryElement(13)]
        public int LastBolusAmount { get; set; }

        [BinaryElement(17)]
        public int LastBolusTime { get; set; }


        private DateTime? _lastBolusDateTime;
        public DateTime? LastBolusDateTime
        {
            get
            {
                if (!_lastBolusDateTime.HasValue)
                {
                    _lastBolusDateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0).AddSeconds(LastBolusTime);
                }
                return _lastBolusDateTime;

            }
            set { _lastBolusDateTime = value; }
        }

        [BinaryElement(21)]
        public Int16 LastBolusReference { get; set; }

        [BinaryElement(23)]
        public byte ActiveBasalPattern { get; set; }

        [BinaryElement(24)]
        public int NormalBasalRaw { get; set; }

        [BinaryElement(28)]
        public int TempBasal { get; set; }

        [BinaryElement(32)]
        public byte TempBasalPercentage { get; set; }

        [BinaryElement(33)]
        public Int16 TempBasalMinutesRemaining { get; set; }

        [BinaryElement(35)]
        public int BasalUnitsDeliveredTodayRaw { get; set; }

        [BinaryElement(39)]
        public byte BatteryPercentage { get; set; }

        [BinaryElement(40)]
        public int ReservoirAmountRaw { get; set; }


        [BinaryElement(44)]
        public byte InsulinHours { get; set; }

        [BinaryElement(45)]
        public byte InsulinMinutes { get; set; }


        [BinaryElement(46)]
        public InsulinDataType ActiveInsulin { get; set; }

        [BinaryElement(50)]
        public Int16 SgvRaw { get; set; }

        public int Sgv { get; set; }


        [BinaryElement(52)]
        public DateTimeDataType SgvDateTime { get; set; }

        [BinaryElement(60)]
        public byte LowSuspendActive { get; set; }

        [BinaryElement(61)]
        public byte CgmTrend { get; set; }

        [BinaryElement(62)]
        public byte SensorStatusFlag { get; set; }

        [BinaryElement(63)]
        public byte Unknown3 { get; set; }

        [BinaryElement(64)]
        public Int16 SensorCalibrationMinutesRemaining { get; set; }


        private DateTime? _sensorCalibrationDateTime;
        public DateTime? SensorCalibrationDateTime
        {
            get
            {
                if (!_sensorCalibrationDateTime.HasValue && this.SensorCalibrationMinutesRemaining > 0)
                {
                    if (this.SgvDateTime.DateTime.HasValue)
                    {
                        _sensorCalibrationDateTime = this.SgvDateTime.DateTime.Value.AddMinutes(SensorCalibrationMinutesRemaining);
                    }
                    
                }
                return _sensorCalibrationDateTime;
            }
            set { _sensorCalibrationDateTime = value; }
        }

        [BinaryElement(66)]
        public byte SensorBatteryRaw { get; set; }

        private int _sensorBattery;
        public int SensorBattery
        {
            get
            {

                if (SensorBatteryRaw == 0x3F) _sensorBattery = 100;
                else if (SensorBatteryRaw == 0x2B) _sensorBattery = 73;
                else if (SensorBatteryRaw == 0x27) _sensorBattery = 47;
                else if (SensorBatteryRaw == 0x23) _sensorBattery = 20;
                else if (SensorBatteryRaw == 0x10) _sensorBattery = 0;
                else _sensorBattery = 0;
                return _sensorBattery;
            }
            set { _sensorBattery = value; }
        }

        [BinaryElement(67)]
        public Int16 SensorRateOfChangeRaw { get; set; }

        [BinaryElement(69)]
        public byte BolusWizardRecent { get; set; }

        [BinaryElement(70)]
        public Int16 BolusWizardBGLRaw { get; set; }

        public int BolusWizardBGL { get; set; }

        [BinaryElement(72)]
        public Int16 Alert { get; set; }

        [BinaryElement(74)]
        public DateTimeDataType AlertDateTime { get; set; }

      
        [BinaryElement(82)]
        public byte[] Unknown6 { get; set; }


        private double _sgvMmol;
        public double SgvMmol
        {
            get
            {

                _sgvMmol = Math.Round(((double)this.Sgv / 18.01559), 1);
                return _sgvMmol;
            }
            set { _sgvMmol = value; }
        }

        private double _normalBasal;
        public double NormalBasal
        {
            get
            {
                _normalBasal = ((double)this.NormalBasalRaw / 10000);
                return _normalBasal;
            }
            set { _normalBasal = value; }
        }
        //public double ActiveInsulin { get { return ((double)this.ActiveInsulinRawConvert / 10000); } }

        //public double RateOfChange
        //{
        //    get
        //    {

        //        if (this.RateOfChange != 0)
        //        {
        //            return ((double)this.RateOfChange / 100);
        //        }
        //        return 0;
        //    }
        //}
        private double _bolusEstimate;
        public double BolusEstimate
        {
            get
            {
                _bolusEstimate = ((double)this.LastBolusAmount / 10000);
                return _bolusEstimate;
            }
            set { _bolusEstimate = value; }
        }
        private double _basalUnitsDeliveredToday;
        public double BasalUnitsDeliveredToday
        {
            get
            {
                _basalUnitsDeliveredToday = ((double)this.BasalUnitsDeliveredTodayRaw / 10000);
                return _basalUnitsDeliveredToday;
            }
            set { _basalUnitsDeliveredToday = value; }
        }


        //public double CalibrationFactor { get { return ((double)(this.CalibrationFactorRaw & 0x0000ffff )/ 10000); } }
        private double _reservoirAmount;
        public double ReservoirAmount
        {
            get
            {
                _reservoirAmount = ((double)this.ReservoirAmountRaw / 10000);
                return _reservoirAmount;
            }
            set { _reservoirAmount = value; }
        }
        private PumpStatus _status;
        public PumpStatus Status
        {
            get
            { _status= new PumpStatus(this.StatusFlag);
                return _status;
            }
            set { _status = value; }
        }
        private SensorStatus _sensorStatus;
        public SensorStatus SensorStatus
        {
            get
            { _sensorStatus= new SensorStatus(this.SensorStatusFlag);
                return _sensorStatus;
            }
            set { _sensorStatus = value; }
        }


        private Alerts _alertName;
       
        public Alerts AlertName { get { _alertName=(Alerts)this.Alert;
                return _alertName;
            }
            set { _alertName = value; }
        }


        //public double SgvDateTimeEpoch
        //{
        //    get
        //    {
        //        if (this.SgvDateTime.HasValue)
        //        {
        //            DateTimeOffset utcTime2 = this.SgvDateTime.Value;
        //            return utcTime2.ToUnixTimeMilliseconds();
        //        }
        //        return 0;
        //    }
        //}
        
        public byte[] AllBytes { get; set; }

        public DateTime PumpStatusDateTime { get; set; }

        // public DateTime LocalDateTime { get; set; }
        //public TimeSpan LocalDateTimePumpDateTimeDifference { get; set; }
        public PumpStatusMessage()
        {
            //LocalDateTime = DateTime.Now;

        }
        public string BytesAsString { get; set; }
        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.AddStatus(this);
            this.AllBytes = bytes;
            this.BytesAsString = BitConverter.ToString(bytes);
            //if (this.SgvDateTime.HasValue)
            //{
            //    this.LocalDateTimePumpDateTimeDifference = this.LocalDateTime.Subtract(this.SgvDateTime.Value);
            //}

            //this.ActiveInsulinRawConvert = this.ActiveInsulinRaw & 0x0000ffff;
            //if (this.BolusEstModifiedByUser==1)
            //{
            //    this.ActiveInsulinRawConvert += 0x0000ffff + 1;
            //}
            //errors where sgv >= 769
            this.Sgv = this.SgvRaw & 0x0000ffff;
            this.BolusWizardBGL = this.BolusWizardBGLRaw & 0x0000ffff;

            if (settings.PumpTime.PumpDateTime.HasValue)
            {
                PumpStatusDateTime = settings.PumpTime.PumpDateTime.Value;
            }
        }




        public override string ToString()
        {
            return string.Format("{0} ({1}/{2})", this.SgvDateTime?.ToString(), this.Sgv, this.SgvMmol);
        }
    }
    [Serializable]
    public class SensorStatus
    {
        public bool Calibrating { get; set; }
        public bool CalibrationComplete { get; set; }
        public bool Exception { get; set; }

        public SensorStatus(byte status)
        {
            Calibrating = (status & 0x01) != 0x00;
            CalibrationComplete = (status & 0x02) != 0x00;
            Exception = (status & 0x04) != 0x00;
        }

        public override string ToString()
        {
            List<string> names = new List<string>();
            if (Calibrating) names.Add(nameof(Calibrating));
            if (CalibrationComplete) names.Add(nameof(CalibrationComplete));
            if (Exception) names.Add(nameof(Exception));

            return string.Join("/", names);
        }
    }
    [Serializable]
    public class PumpStatus
    {
        public bool Suspended { get; set; }
        public bool BolusingNormal { get; set; }
        public bool BolusingSquare { get; set; }
        public bool BolusingDual { get; set; }


        public bool DeliveringInsulin { get; set; }
        public bool CgmActive { get; set; }
        public bool TempBasalActive { get; set; }

        public PumpStatus(byte status)
        {
            Suspended = (status & 0x01) != 0x00;
            BolusingNormal = (status & 0x02) != 0x00;
            BolusingSquare = (status & 0x04) != 0x00;
            BolusingDual = (status & 0x08) != 0x00;
            DeliveringInsulin = (status & 0x10) != 0x00;
            TempBasalActive = (status & 0x20) != 0x00;
            CgmActive = (status & 0x40) != 0x00;

            //cgmCalibrating = (cgmStatus & 0x01) != 0x00;
            //cgmCalibrationComplete = (cgmStatus & 0x02) != 0x00;
            //cgmException = (cgmStatus & 0x04) != 0x00;

        }

        public override string ToString()
        {
            List<string> names = new List<string>();
            if (Suspended) names.Add(nameof(Suspended));
            if (BolusingNormal) names.Add(nameof(BolusingNormal));
            if (BolusingSquare) names.Add(nameof(BolusingSquare));
            if (BolusingDual) names.Add(nameof(BolusingDual));
            if (DeliveringInsulin) names.Add(nameof(DeliveringInsulin));
            if (CgmActive) names.Add(nameof(CgmActive));
            if (TempBasalActive) names.Add(nameof(TempBasalActive));




            return string.Join("/", names);
        }
    }
}
