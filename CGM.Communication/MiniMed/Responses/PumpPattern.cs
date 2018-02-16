using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace CGM.Communication.MiniMed.Responses
{
    [BinaryType(IsLittleEndian = false)]
    public class PumpPattern : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public byte PatternNumber { get; set; }

        [BinaryElement(1, Length = 1)]
        public byte PatternCount { get; set; }

        [BinaryElement(22)]
        public byte[] Unknown { get; set; }

        public List<PumpPatternDetail> PatternDetails { get; set; } = new List<PumpPatternDetail>();

        public byte[] AllBytes { get; set; }

        public string BytesAsString { get; set; }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            this.AllBytes = bytes;
            this.BytesAsString = BitConverter.ToString(AllBytes);
            if (bytes.Length > 6)
            {


                Serializer serializer = new Serializer(settings);
                PatternDetails = new List<PumpPatternDetail>();

                int startIndex = 4;
                int loopInt = 5;
                for (int i = 0; i < PatternCount; i++)
                {
                    int arraystart = (i * loopInt) + startIndex;
                    byte[] array = this.AllBytes.ToList().GetRange(arraystart, 3).ToArray();
                    var detail = serializer.Deserialize<PumpPatternDetail>(array);
                    PatternDetails.Add(detail);

                }

                for (int i = 0; i < PatternDetails.Count; i++)
                {
                    if (i== PatternDetails.Count-1)
                    {
                        //last one
                        PatternDetails[i].End = new TimeSpan(24, 0, 0);
                    }
                    else
                    {
                        PatternDetails[i].End = PatternDetails[i + 1].Start;
                    }
                    
                }

                settings.PumpSettings.PumpPatterns.Add(this);
            }
        }

        public override string ToString()
        {
            return $"Patterns: ({this.PatternDetails.Count})";
        }

    }

    [BinaryType(IsLittleEndian = false)]
    public class PumpPatternDetail : IBinaryType, IBinaryDeserializationSetting
    {
        [BinaryElement(0)]
        public Int16 BasalRate { get; set; }

        [BinaryElement(2, Length = 1)]
        public byte MinutesIntervalStart { get; set; }

        public TimeSpan Start { get { return TimeSpan.FromMinutes(MinutesIntervalStart * 30); } }

        public double UnitsPerHour { get { return (double)this.BasalRate / 10000; } }

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
            return string.Format("{0}-{1} ({2})", Start, End, UnitsPerHour);
        }
    }
}
