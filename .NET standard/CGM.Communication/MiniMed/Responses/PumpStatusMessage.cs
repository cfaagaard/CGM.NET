using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGM.Communication.MiniMed.Responses
{
    public enum StatusFlag
    {
        No_sgv=16
    }


    [BinaryType(IsLittleEndian = false)]
    public class PumpStatusMessage : IBinaryType, IBinaryDeserializationSetting
    {



        [BinaryElement(0)]
        public byte StatusFlag { get; set; }

        [BinaryElement(1)]
        public int NowBolusingAmountDelivered { get; set; }

        [BinaryElement(5)]
        public byte[] Unknown1 { get; set; }

        [BinaryElement(10)]
        public byte NowBolusingMinutesRemaining { get; set; }

        [BinaryElement(11)]
        public Int16 NowBolusingReference { get; set; }

        [BinaryElement(13)]
        public int LastBolusAmount { get; set; }

        [BinaryElement(17)]
        public int LastBolusTime { get; set; }

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

        //[BinaryElement(46)]
        //public Int32 ActiveInsulinRaw { get; set; }

        [BinaryElement(46)]
        public byte Unknown2 { get; set; }

        [BinaryElement(47)]
        public byte BolusEstModifiedByUser { get; set; }

        [BinaryElement(48)]
        public UInt16 ActiveInsulinRaw { get; set; }

        public int ActiveInsulinRawConvert { get; set; }

        [BinaryElement(50)]
        public Int16 SgvRaw { get; set; }

        public int Sgv { get; set; }

        [BinaryElement(52)]
        public int SgvDateTimeRtc { get; set; }

        [BinaryElement(56)]
        public int SgvDateTimeOffSet { get; set; }

        [BinaryElement(60)]
        public byte LowSuspendActive { get; set; }

        [BinaryElement(61)]
        public byte CgmTrend { get; set; }

        [BinaryElement(62)]
        public byte SensorStatus { get; set; }

        [BinaryElement(63)]
        public byte Unknown3 { get; set; }

        [BinaryElement(64)]
        public Int16 SensorCalibrationMinutesRemaining { get; set; }

        [BinaryElement(66)]
        public byte SensorBattery_maybe { get; set; }

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
        public int AlertRtc { get; set; }

        [BinaryElement(78)]
        public int AlertOffset { get; set; }

        [BinaryElement(82)]
        public byte[] Unknown7 { get; set; }

        //[BinaryElement(92)]
        //public Int16 UnknownNumber { get; set; }

        //[BinaryElement(94)]
        //public byte[] Unknown8 { get; set; }

        //Calculated properties
        public DateTime? SgvDateTime { get { return DateTimeExtension.GetDateTime(this.SgvDateTimeRtc, this.SgvDateTimeOffSet); } }

        public DateTime? AlertDateTime
        {
            get
            {
                if (this.AlertRtc != 0)
                {
                    return DateTimeExtension.GetDateTime(this.AlertRtc, this.AlertOffset);
                }
                else
                {
                    return null;
                }

            }
        }
        public double SgvMmol { get { return Math.Round(((double)this.Sgv / 18.01559), 1); } }

        public double NormalBasal { get { return ((double)this.NormalBasalRaw / 10000); } }
        public double ActiveInsulin { get { return ((double)this.ActiveInsulinRawConvert / 10000); } }

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

        public double BolusEstimate
        {
            get
            {
                return ((double)this.LastBolusAmount / 10000);
            }
        }

        public double BasalUnitsDeliveredToday { get { return ((double)this.BasalUnitsDeliveredTodayRaw / 10000); } }


        //public double CalibrationFactor { get { return ((double)(this.CalibrationFactorRaw & 0x0000ffff )/ 10000); } }

        public double ReservoirAmount { get { return ((double)this.ReservoirAmountRaw / 10000); } }
        public PumpStatus Status
        {
            get
            { return new PumpStatus(this.StatusFlag); }
        }

        public SgvTrend CgmTrendName { get { return TrendConvert(this.CgmTrend); } }

        public Alerts AlertName { get { return (Alerts)this.Alert; } }


        public double SgvDateTimeEpoch
        {
            get
            {
                if (this.SgvDateTime.HasValue)
                {
                    DateTimeOffset utcTime2 = this.SgvDateTime.Value;
                    return utcTime2.ToUnixTimeMilliseconds();
                }
                return 0;
            }
        }

        public byte[] AllBytes { get; set; }


        public DateTime LocalDateTime { get; set; }
        //public TimeSpan LocalDateTimePumpDateTimeDifference { get; set; }
        public PumpStatusMessage()
        {
            LocalDateTime = DateTime.Now;

        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            settings.AddStatus(this);
            this.AllBytes = bytes;

            //if (this.SgvDateTime.HasValue)
            //{
            //    this.LocalDateTimePumpDateTimeDifference = this.LocalDateTime.Subtract(this.SgvDateTime.Value);
            //}

            this.ActiveInsulinRawConvert = this.ActiveInsulinRaw & 0x0000ffff;
            if (this.BolusEstModifiedByUser==1)
            {
                this.ActiveInsulinRawConvert += 0x0000ffff + 1;
            }
            //errors where sgv >= 769
            this.Sgv= this.SgvRaw & 0x0000ffff;
            this.BolusWizardBGL= this.BolusWizardBGLRaw & 0x0000ffff;


        }

        private SgvTrend TrendConvert(byte messageByte)
        {
            if (this.Sgv!=0)
            {
                switch (messageByte)
                {
                    case (byte)0x60:
                        return SgvTrend.Flat;
                    case (byte)0xc0:
                        return SgvTrend.DoubleUp;
                    case (byte)0xa0:
                        return SgvTrend.SingleUp;
                    case (byte)0x80:
                        return SgvTrend.FortyFiveUp;
                    case (byte)0x40:
                        return SgvTrend.FortyFiveDown;
                    case (byte)0x20:
                        return SgvTrend.SingleDown;
                    case (byte)0x00:
                        return SgvTrend.DoubleDown;
                    default:
                        return SgvTrend.NotComputable;
                }
            }
            return SgvTrend.NotSet;
  
        }


        public override string ToString()
        {
            return string.Format("{0} ({1}/{2})", this.SgvDateTime?.ToString(), this.Sgv, this.SgvMmol);
        }
    }


    public class PumpStatus
    {
        public bool Suspended { get; set; }
        public bool Bolusing { get; set; }
        public bool DeliveringInsulin { get; set; }
        public bool CgmActive { get; set; }
        public bool TempBasalActive { get; set; }

        public PumpStatus(byte status)
        {
            this.Suspended = (status & 1) != 0x00;
            this.Bolusing = (status & 2) != 0x00;
            this.DeliveringInsulin = (status & 16) != 0x00;
            this.TempBasalActive = (status & 32) != 0x00;
            this.CgmActive = (status & 64) != 0x00;
        }

        public override string ToString()
        {
            List<string> names = new List<string>();
            if (Suspended) names.Add(nameof(Suspended));
            if (Bolusing) names.Add(nameof(Bolusing));
            if (DeliveringInsulin) names.Add(nameof(DeliveringInsulin));
            if (CgmActive) names.Add(nameof(CgmActive));
            if (TempBasalActive) names.Add(nameof(TempBasalActive));
            return string.Join("/", names);
        }
    }
}
