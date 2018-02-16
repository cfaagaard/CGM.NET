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
    public class BolusWizardSensitivityFactorsResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(2,Length =1)]
        public byte Count { get; set; }

        [BinaryElement(3)]
        [BinaryElementList(CountProperty = nameof(Count), Type = typeof(BolusWizardSensitivityFactorDetail), ByteSize = 5)]
        public List<BolusWizardSensitivityFactorDetail> BolusWizardSensitivityFactorDetails { get; set; } = new List<BolusWizardSensitivityFactorDetail>();

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            //settings.DeviceString = this;
            //calculate End....
            for (int i = 0; i < BolusWizardSensitivityFactorDetails.Count; i++)
            {
                if (i == BolusWizardSensitivityFactorDetails.Count - 1)
                {
                    //last one
                    BolusWizardSensitivityFactorDetails[i].End = new TimeSpan(24, 0, 0);
                }
                else
                {
                    BolusWizardSensitivityFactorDetails[i].End = BolusWizardSensitivityFactorDetails[i + 1].Start;
                }

            }
            settings.PumpSettings.BolusWizardSensitivityFactors = this;
        }

        public override string ToString()
        {
            return $"BolusWizardBGTargets: ({this.BolusWizardSensitivityFactorDetails.Count})";
        }
    }

    [BinaryType(IsLittleEndian = false)]
    public class BolusWizardSensitivityFactorDetail : IBinaryType, IBinaryDeserializationSetting
    {
        //this should be test with the pump.....
        [BinaryElement(0, Length = 1)]
        public byte MinutesIntervalStart { get; set; }

        [BinaryElement(2, Length = 2)]
        public Int16 Amount { get; set; }



        public TimeSpan Start { get { return TimeSpan.FromMinutes(MinutesIntervalStart * 30); } }

        public TimeSpan End { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
           // throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} ({2})", Start, End, Amount);
        }
    }

    }
