using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class BolusWizardBGTargetsResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(2,Length =1)]
        public byte Count { get; set; }

        [BinaryElement(3)]
        [BinaryElementList(CountProperty = nameof(Count), Type = typeof(BolusWizardBGTargetDetail), ByteSize = 9)]
        public List<BolusWizardBGTargetDetail> BolusWizardBGTargetDetails { get; set; } = new List<BolusWizardBGTargetDetail>();

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //settings.DeviceString = this;
            //calculate End....
            for (int i = 0; i < BolusWizardBGTargetDetails.Count; i++)
            {
                if (i == BolusWizardBGTargetDetails.Count - 1)
                {
                    //last one
                    BolusWizardBGTargetDetails[i].End = new TimeSpan(24, 0, 0);
                }
                else
                {
                    BolusWizardBGTargetDetails[i].End = BolusWizardBGTargetDetails[i + 1].Start;
                }

            }
            settings.PumpSettings.BolusWizardBGTargets = this;
        }

        public override string ToString()
        {
            return $"BolusWizardBGTargets: ({this.BolusWizardBGTargetDetails.Count})";
        }
    }

    [BinaryType(IsLittleEndian = false)]
    public class BolusWizardBGTargetDetail : IBinaryType, IBinaryDeserializationSetting
    {
        //this should be test with the pump.....
        [BinaryElement(0,Length =1)]
        public byte MinutesIntervalStart { get; set; }

        [BinaryElement(2,Length =2)]
        public Int16 High { get; set; }

        //[BinaryElement(4)]
        //public Int16 MinutesIntervalEnd { get; set; }

        [BinaryElement(6,Length =2)]
        public Int16 Low { get; set; }

        public TimeSpan Start { get { return TimeSpan.FromMinutes(MinutesIntervalStart * 30); } }

        public TimeSpan End { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
           // throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} ({2}-{3})", Start, End, Low,High);
        }
    }

    }
