using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Communication.MiniMed.Responses
{

    [BinaryType(IsLittleEndian = false)]
    public class PumpCarbRatioResponse : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public Int16 Unknown { get; set; }

        [BinaryElement(2)]
        public byte Count { get; set; }

        //[BinaryElement(22)]
        //public byte[] Unknown { get; set; }
        [BinaryElement(3)]
        [BinaryElementList(CountProperty =nameof(Count),Type =typeof(PumpCarbRatioDetail),ByteSize =9)]
        public List<PumpCarbRatioDetail> PumpCarbRatioDetails { get; set; } = new List<PumpCarbRatioDetail>();

        public byte[] AllBytes { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.BytesAsString = BitConverter.ToString(AllBytes);
            //calculate End....
            for (int i = 0; i < PumpCarbRatioDetails.Count; i++)
            {
                if (i == PumpCarbRatioDetails.Count - 1)
                {
                    //last one
                    PumpCarbRatioDetails[i].End = new TimeSpan(24, 0, 0);
                }
                else
                {
                    PumpCarbRatioDetails[i].End = PumpCarbRatioDetails[i + 1].Start;
                }

            }

            settings.PumpSettings.CarbRatio =this;
        }

        public override string ToString()
        {
            return $"CarbRatio: ({this.PumpCarbRatioDetails.Count})";
        }

    }

    [BinaryType(IsLittleEndian = false)]
    public class PumpCarbRatioDetail : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte[] Unknown { get; set; }

        [BinaryElement(3)]
        public byte GuRaw { get; set; }

        [BinaryElement(4)]
        public byte[] Unknown1 { get; set; }

        [BinaryElement(8)]
        public byte MinutesIntervalStart { get; set; }

        public TimeSpan Start { get { return TimeSpan.FromMinutes(MinutesIntervalStart * 30); } }

        public double Gu { get { return (double)this.GuRaw / 10; } }

        public TimeSpan End { get; set; }
        public byte[] AllBytes { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.BytesAsString = BitConverter.ToString(AllBytes);
        }


        public override string ToString()
        {
            return string.Format("{0}-{1} ({2})", Start, End, Gu);
        }
    }
}
