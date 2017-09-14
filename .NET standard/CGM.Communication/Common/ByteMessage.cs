using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace CGM.Communication.Common
{
    public class ByteMessage
    {
        private SerializerSession _settings;
        public AstmStart MedtronicMessage { get; set; }
        public List<ByteFrame> Frames { get; set; } = new List<ByteFrame>();

        public byte[] Bytes { get; set; }

        public string BytesAsString
        {
            get
            {
                if (Bytes != null && Bytes.Length > 0)
                {
                    return BitConverter.ToString(Bytes);
                }
                return "";
            }
        }

        public MessageAnalysis Analysis { get; set; }

        public bool EndOfMessage { get; set; }

        public ByteMessage(SerializerSession settings)
        {
            _settings = settings;
        }

        public void AddFrame(ByteFrame frame)
        {
            Frames.Add(frame);
            if (!frame.MoreBytes)
            {
                CreateMedtronicMessage();
                EndOfMessage = true;
            }
        }

        public bool CanAdd(ByteFrame frame)
        {
            if (this.Frames.Count == 0)
            {
                return true;
            }
            else
            {
                return this.Frames.Last().Bytes[3] == frame.Bytes[3];
            }
        }

        private byte[] MergeFrameBytes()
        {
            List<byte> newbytes = new List<byte>();
            List<byte> start = new List<byte>();


            foreach (var item in this.Frames)
            {
                List<byte> framebytes = new List<byte>();
                framebytes.AddRange(item.Bytes);
                if (start.Count==0)
                {
                    start.AddRange(framebytes.GetRange(0, 5));
                }
               
                if (item.MoreBytes)
                {
                    newbytes.AddRange(framebytes.GetRange(5, framebytes.Count - 5));
                }
                else
                {
                    byte length = framebytes[4];
                    newbytes.AddRange(framebytes.GetRange(5, length));
                }
            }
            newbytes.InsertRange(0,start);
            newbytes[4] = (byte)(newbytes.Count - 5);

            return newbytes.ToArray();
        }

        private void CreateMedtronicMessage()
        {
            Common.Serialize.Serializer serializer = new Common.Serialize.Serializer(_settings);
            this.Bytes = MergeFrameBytes();
            this.MedtronicMessage = serializer.Deserialize<AstmStart>(this.Bytes);
           
        }
        public override string ToString()
        {
            return string.Format("({0})", this.Frames.First().Framenumber.ToString()) + MedtronicMessage.ToString();
        }

        public void Analyze()
        {
            Analysis = new MessageAnalysis(MedtronicMessage.ToString());
            AnalyzeType(MedtronicMessage);
        }

        private void AnalyzeType(object obj)
        {
            var properties = obj.GetType().GetRuntimeProperties();//.GetTypeInfo().();//.DeclaredProperties;

            foreach (var property in properties)
            {

                object value = property.GetValue(obj);

                MessageAnalysisDetail ma = new MessageAnalysisDetail();
                ma.Property = property;

                var element = (BinaryElement)property.GetCustomAttribute(typeof(BinaryElement));
                if (element != null)
                {
                    ma.Element = element;
                }
                if (value != null)
                {
                    bool containssubtype = value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryType));
                    if (containssubtype)
                    {
                        AnalyzeType(property.GetValue(obj));
                    }
                    else
                    {
                        ma.Value = value;
                        Analysis.Details.Add(ma);
                    }
                }
                else
                {
                    Analysis.Details.Add(ma);
                }



            }
        }
    }

    public class MessageAnalysis
    {
        public string Title { get; set; }
        public List<MessageAnalysisDetail> Details { get; set; } = new List<MessageAnalysisDetail>();

        public MessageAnalysis(string title)
        {
            this.Title = title;
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class MessageAnalysisDetail
    {
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }

        public BinaryElement Element { get; set; }

        public MessageAnalysisFacade Facade { get { return new MessageAnalysisFacade() { ElementNumber = Element?.FieldOffset, Name = Property.Name, Value = this.Value, NameType = Property.PropertyType.Name }; } }

    }

    public class MessageAnalysisFacade
    {
        public int? ElementNumber { get; set; }
        public string Name { get; set; }
        public string NameType { get; set; }
        public object Value { get; set; }


    }
}
