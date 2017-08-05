using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Interfaces;
using CGM.Communication.Patterns;

namespace CGM.Communication.Common.Serialize
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BinaryType : System.Attribute
    {
        public bool IsLittleEndian { get; set; } = true;
        public bool IsEncrypted { get; set; } = false;

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class BinaryPropertyValueTransfer : System.Attribute
    {
        public string ParentPropertyName { get; set; }
        public string ChildPropertyName { get; set; }
    }

        [AttributeUsage(AttributeTargets.Property)]
    public class BinaryElement : System.Attribute
    {
        public int FieldOffset { get; set; }
        public bool Encrypt { get; set; } = false;

        public int Length { get; set; } = -1;

        public BinaryElement(int fieldOffset, bool isLittleEndian)
        {
        }

        public BinaryElement(int fieldOffset) : this(fieldOffset, true)
        {
            FieldOffset = fieldOffset;
        }
    }

    public class BinaryElementList:System.Attribute
    {
        public int ByteSize { get; set; }
        public Type Type { get; set; }

        public string CountProperty { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Evaluate : System.Attribute
    {
        public string Path { get; set; }
        public object Value { get; set; }

    }




    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MessageType : Attribute
    {
        public string  Path { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
        public int LengthEquals { get; set; } = 0;
        public IReportPattern ReportPattern { get; set; }

        public Evaluate PropertyEval { get; set; }
        public MessageType(Type type, Type reportPatternType):this(type)
        {
            this.ReportPattern = (IReportPattern)Activator.CreateInstance(reportPatternType);
           
        }

        public MessageType(Type type)
        {
            this.Type = type;
        }

        public MessageType(Type type, string path, object value) : this(type)
        {
            this.Path = path;
            this.Value = value;
        }


    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MessageTypeCollection :  Attribute
    {
        public Type Type { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BinaryElementLogic : System.Attribute
    {
        public int Order { get; set; } = 0;
        public abstract void SetValue(List<byte> bytes, int fieldOffset, PropertyInfo propertyInfo, Object value, SerializerSession settings);
    }


    public class BinaryElementLogicEncrypt : BinaryElementLogic
    {
        public bool ForEncryption { get; set; } = true;
        public int Length { get; set; }
        public BinaryElementLogicEncrypt()
        {
            Order = 0;
        }
        public override void SetValue(List<byte> bytes, int fieldOffset, PropertyInfo propertyInfo, Object value, SerializerSession settings)
        {

            //ForEncryption = bytes[1] == 0x00;

            if (settings.EncryptKey == null || settings.EncryptIV == null)
            {
                throw new Exception("Missing encryptKey/IV in serializationsettings.");
            }

            var temp = bytes.ToList().GetRange(fieldOffset, Length).ToArray();

            var crp = temp.CryptMessage(ForEncryption, settings.EncryptKey, settings.EncryptIV);

            for (int i = 0; i < crp.Length; i++)
            {
                bytes[fieldOffset + i] = crp[i];
            }
        }
    }



    public class BinaryElementLogicCrc16Ciit : BinaryElementLogic
    {
        public BinaryElementLogicCrc16Ciit()
        {
            Order = 1;
        }

        public override void SetValue(List<byte> bytes, int fieldOffset, PropertyInfo propertyInfo, Object value, SerializerSession settings)
        {
            var element = (BinaryType)value.GetType().GetTypeInfo().GetCustomAttribute(typeof(BinaryType));

            if (fieldOffset==1000)
            {
                fieldOffset = bytes.Count - 2;
            }

            // do not get the crc-field
            var temp = bytes.GetRange(0, bytes.Count - 2);

            var crc = Crc16Ccitt.CRC16CCITT(temp.ToArray(), 0xffff, 0x1021, temp.Count);

            var crcbytes = BitConverter.GetBytes((short)crc).ToList();

            if (!element.IsLittleEndian)
            {
                crcbytes = crcbytes.ToArray().Reverse().ToList();
            }

            bytes[fieldOffset] = crcbytes[0];
            bytes[fieldOffset + 1] = crcbytes[1];
            propertyInfo.SetValue(value, crcbytes.ToArray());

            //    if (element.IsLittleEndian)
            //{
            //    bytes[fieldOffset] = crcbytes[0];
            //    bytes[fieldOffset + 1] = crcbytes[1];
            //    propertyInfo.SetValue(value, crcbytes);
            //}
            //else
            //{
            //    bytes[fieldOffset] = crcbytes[1];
            //    bytes[fieldOffset + 1] = crcbytes[0];
            //    propertyInfo.SetValue(value, crcbytes);
            //}

        }
    }


    public class BinaryElementLogicByteSum : BinaryElementLogic
    {
        public int From { get; set; } = 0;

        public int Remove { get; set; }

        public BinaryElementLogicByteSum()
        {
            Order = 2;
        }
        public override void SetValue(List<byte> bytes, int fieldOffset, PropertyInfo propertyInfo, Object value, SerializerSession settings)
        {
            if (fieldOffset <= bytes.Count)
            {
                var temp = bytes.ToList().GetRange(From, bytes.Count - From).ToArray();
                bytes[fieldOffset] = temp.OneByteSum();
            }

        }



    }


    public class BinaryElementLogicLength : BinaryElementLogic
    {
        public int Add { get; set; }
        public int From { get; set; }
        public override void SetValue(List<byte> bytes, int fieldOffset, PropertyInfo propertyInfo, Object value, SerializerSession settings)
        {
            var len = (bytes.Count - From) + Add;
            bytes[fieldOffset] = BitConverter.GetBytes(len)[0];
        }

    }

    public interface IBinaryDeserializationSetting
    {
        void OnDeserialization(byte[] bytes, SerializerSession settings);

    }

    public interface IBinarySerializationSetting
    {
        void OnSerialization(List<byte> bytes, SerializerSession settings);

    }

    public interface IBinaryType
    {

    }

    //public interface IBinaryTypeSerializable<T>
    //{
    //    byte[] Serialize(T type);

    //    T Deserialize(byte[] bytes);
    //}
}
