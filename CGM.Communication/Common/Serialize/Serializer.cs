using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Requests;
using CGM.Communication.MiniMed;
using CGM.Communication.Interfaces;
using System.Collections;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;

namespace CGM.Communication.Common.Serialize
{

    public class SerializeInfoElement
    {
        public BinaryElement Element { get; set; }
        public PropertyInfo Info { get; set; }


    }

    public class SerializeInfo<T>
    {

        private byte[] _bytes;
        private T _objectClass;
        public Dictionary<string, SerializeInfoElement> InfoElements { get; set; } = new Dictionary<string, SerializeInfoElement>();

        public List<PropertyInfo> PropertyInfos { get; set; } = new List<PropertyInfo>();

        public List<SerializeInfoElement> InfoElementOrderedList { get; set; } = new List<SerializeInfoElement>();


        public SerializeInfo(T objectClass)
        {
            _objectClass = objectClass;
            SetDictionary();
        }
        public SerializeInfo(byte[] bytes)
        {
            _bytes = bytes;
            SetDictionary();
            SetAttributes();
        }



        private void SetDictionary()
        {
            var properties = typeof(T).GetRuntimeProperties();
            foreach (var property in properties)
            {
                var element = (BinaryElement)property.GetCustomAttribute(typeof(BinaryElement));
                if (element != null)
                {
                    if (element.Direction == DirectionEnum.Reverse)
                    {
                        if (_bytes == null)
                        {
                            element.FieldOffset = 1000;
                        }
                        else
                        {
                            element.FieldOffset = _bytes.Length - element.Length - element.FieldOffset;
                        }

                    }
                    var el = new SerializeInfoElement() { Element = element, Info = property };

                    InfoElements.Add(property.Name, el);
                }
                PropertyInfos.Add(property);
            }

            InfoElementOrderedList.AddRange(InfoElements.Select(e => e.Value).OrderBy(e => e.Element.FieldOffset));

        }


        private void SetAttributes()
        {

            for (int i = 0; i < InfoElementOrderedList.Count; i++)
            {
                var current = InfoElementOrderedList[i];

                if (i + 1 < InfoElementOrderedList.Count)
                {
                    if (InfoElementOrderedList[i + 1].Element.FieldOffset == 1000)
                    {
                        var next = InfoElementOrderedList[i + 1];

                        current.Element.Length = (_bytes.Length - current.Element.FieldOffset) - next.Element.Length;
                        next.Element.FieldOffset = current.Element.FieldOffset + current.Element.Length;
                    }
                }
            }
        }
    }

    public class Serializer
    {
        private ILogger Logger = ApplicationLogging.CreateLogger<Serializer>();
        private SerializerSession _session = new SerializerSession();


        public Serializer(SerializerSession session)
        {
            _session = session;
        }

        class AttributeList
        {
            public BinaryElementLogic Logic { get; set; }
            public PropertyInfo PropInfo { get; set; }
        }
        private List<AttributeList> GetLogics<T>(T s)
        {
            List<AttributeList> dic = new List<AttributeList>();

            //SerializeInfo<T> serializeInfo = new SerializeInfo<T>(null);

            //foreach (var item in serializeInfo.InfoElements.Values)
            //{
            //    dic.Add(new AttributeList() { Logic = item.Element, PropInfo = item.Info });
            //}

            var properties = typeof(T).GetRuntimeProperties();

            foreach (var property in properties)
            {
                var element = (BinaryElementLogic)property.GetCustomAttribute(typeof(BinaryElementLogic));
                if (element != null)
                {
                    dic.Add(new AttributeList() { Logic = element, PropInfo = property });
                }

            }
            return dic;
        }

        private List<byte[]> _byteLevels = new List<byte[]>();

        public byte[] Serialize<T>(T bytes) where T : class, new()
        {
            if (this._session.SessionSystem.PreserveMessages)
            {
                this._session.SessionSystem.Messages.Add(bytes);
            }

            return SerializeInternal<T>(bytes);
        }


        private byte[] SerializeInternal<T>(T s) where T : class
        {

            var classtype = (BinaryType)typeof(T).GetTypeInfo().GetCustomAttribute(typeof(BinaryType));
            if (classtype != null)
            {

                //var dic = GetDic(s);
                //var query = dic.Keys.OrderBy(e => e.FieldOffset);
                List<byte> temp = new List<byte>();
                SerializeInfo<T> serInfo = new SerializeInfo<T>(s);
                bool IsLittleEndian = BitConverter.IsLittleEndian;
                IsLittleEndian = classtype.IsLittleEndian;
  
                foreach (var item in serInfo.InfoElementOrderedList)
                {


                    var property = item.Info;// dic[item];
                    byte[] value = null;
                    var getvalue = property.GetValue(s);
                    if (getvalue != null)
                    {


                        var checktype = getvalue.GetType();

                        if (checktype == typeof(byte))
                        {
                            value = new byte[] { (byte)property.GetValue(s) };
                        }

                        if (checktype == typeof(UInt16))
                        {
                            value = BitConverter.GetBytes((UInt16)property.GetValue(s));
                        }

                        if (checktype == typeof(byte[]))
                        {
                            value = (byte[])property.GetValue(s);

                        }


                        if (checktype == typeof(string))
                        {
                            //value = Encoding.ASCII.GetBytes((string)property.GetValue(s));
                            value = Encoding.UTF8.GetBytes((string)property.GetValue(s));
                        }


                        if (property.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryType)))
                        {
                            var serializerMethodes = typeof(CGM.Communication.Common.Serialize.Serializer).GetRuntimeMethods();
                            var generic0 = serializerMethodes.FirstOrDefault(e => e.Name == "SerializeInternal");
                            var generic = generic0.MakeGenericMethod(property.PropertyType);
                            value = (byte[])generic.Invoke(this, new object[] { getvalue });
                        }


                        if (value == null && getvalue != null)
                        {
                            if (getvalue.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryType)))
                            {
                                var serializerMethodes = typeof(CGM.Communication.Common.Serialize.Serializer).GetRuntimeMethods();
                                var generic0 = serializerMethodes.FirstOrDefault(e => e.Name == "SerializeInternal");
                                var generic = generic0.MakeGenericMethod(checktype);
                                var tempValue = (byte[])generic.Invoke(this, new object[] { getvalue });
                                if (IsLittleEndian == false)
                                {
                                    value = tempValue.Reverse().ToArray();
                                }
                                else
                                {
                                    value = tempValue;
                                }
                            }
                        }



                        if (value != null)
                        {
                            if (IsLittleEndian == false)
                            {
                                value = value.Reverse().ToArray();
                            }

                            if (item.Element.FieldOffset == 1000)
                            {
                                temp.AddRange(value);
                            }
                            else
                            {
                                temp.InsertRange(item.Element.FieldOffset, value);

                            }

                        }
                    }
                }



                var logicAttributes = GetLogics<T>(s);

                if (logicAttributes.Count > 0)
                {
                    var logicOrdered = logicAttributes.OrderBy(e => e.Logic.Order);
                    foreach (var item in logicOrdered)
                    {
                        var element = serInfo.InfoElements.FirstOrDefault(e => e.Key == item.PropInfo.Name).Value;
                        if (element != null)
                        {
                            item.Logic.SetValue(temp, element.Element.FieldOffset, item.PropInfo, s, _session);
                        }
                        //  var element = (BinaryElement)item.PropInfo.GetCustomAttribute(typeof(BinaryElement));

                    }
                }



                if (typeof(T).GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinarySerializationSetting)))
                {
                    ((IBinarySerializationSetting)s).OnSerialization(temp, _session);
                }

                if (classtype.IsEncrypted)
                {
                    if (_session.SessionCommunicationParameters.EncryptKey == null || _session.SessionCommunicationParameters.EncryptIV == null)
                    {
                        throw new Exception("Missing encryptKey/IV in serializationsettings.");
                    }
                    temp = temp.ToArray().Encrypt(_session.SessionCommunicationParameters.EncryptKey, _session.SessionCommunicationParameters.EncryptIV).ToList();
                }

                return temp.ToArray();
            }
            return null;

        }

        //public byte[] Join(List<byte[]> bytes)
        //{




        //        if (bytes != null && bytes.Count > 0)
        //    {
        //        List<byte> newbytes = new List<byte>();
        //        newbytes.AddRange(bytes[0]);
        //        if (bytes.Count > 1)
        //        {
        //            for (int i = 1; i < bytes.Count; i++)
        //            {
        //                List<byte> newlist = new List<byte>(bytes[i]);


        //                var j = newlist.Count - 1;
        //                while (newlist[j] == 0)
        //                {
        //                    --j;
        //                }
        //                var temp = new byte[j + 1];

        //                Array.Copy(newlist.ToArray(), temp, j + 1);

        //                var list = temp.ToList();
        //                list.RemoveRange(0, 5);
        //                newbytes.AddRange(list);
        //            }
        //            newbytes[4] = (byte)(newbytes.Count - 5);
        //        }
        //        var data = newbytes.ToArray();
        //        bool data_found = false;
        //        byte[] new_data = data.Reverse().SkipWhile(point =>
        //        {
        //            if (data_found) return false;
        //            if (point == 0x00) return true; else { data_found = true; return false; }
        //        }).Reverse().ToArray();

        //        return new_data;
        //    }
        //    return null;
        //}



        public T Deserialize<T>(byte[] bytes) where T : class, new()
        {
            _byteLevels = new List<byte[]>();

            T obj = DeserializeInternal<T>(bytes);
            if (this._session.SessionSystem.PreserveMessages)
            {
                this._session.SessionSystem.Messages.Add(obj);
            }
            return obj;
        }

        private T DeserializeInternal<T>(byte[] bytes) where T : class, new()
        {
            _byteLevels.Add(bytes);
            T byteClass = new T();
            var classtype = (BinaryType)typeof(T).GetTypeInfo().GetCustomAttribute(typeof(BinaryType));

            if (classtype != null)
            {

                if (classtype.IsEncrypted)
                {
                    if (_session.SessionCommunicationParameters.EncryptKey == null || _session.SessionCommunicationParameters.EncryptIV == null)
                    {
                        throw new Exception("Missing encryptKey/IV in serializationsettings.");
                    }
                    bytes = bytes.Decrypt(_session.SessionCommunicationParameters.EncryptKey, _session.SessionCommunicationParameters.EncryptIV);
                }


                var bytelist = new List<byte>(bytes);
                SerializeInfo<T> serInfo = new SerializeInfo<T>(bytes);

                for (int i = 0; i < serInfo.InfoElementOrderedList.Count; i++)
                {

                    var current = serInfo.InfoElementOrderedList[i];

                    int start = current.Element.FieldOffset;
                    if (start < bytes.Length)
                    {
                        var property = current.Info;
                        int length = 0;
                        if (current.Element.Length != -1)
                        {
                            length = current.Element.Length;
                        }
                        else
                        {
                            if (i + 1 != serInfo.InfoElementOrderedList.Count)
                            {
                                length = serInfo.InfoElementOrderedList[i + 1].Element.FieldOffset - start;
                            }
                            else
                            {
                                length = bytes.Length - start;
                            }
                            //todo: last one
                            if (bytes.Length < (start + length))
                            {
                                length = bytes.Length - start;
                            }
                        }

                        var value = bytelist.GetRange(start, length);



                        object setvalue = null;

                        bool IsLittleEndian = BitConverter.IsLittleEndian;
                        IsLittleEndian = classtype.IsLittleEndian;

                        List<byte> EndianValue = new List<byte>();
                        if (IsLittleEndian)
                        {
                            EndianValue = value;
                        }
                        else
                        {
                            EndianValue = value.ToArray().Reverse().ToList();
                        }


                        if (value != null)
                        {
                            if (property.PropertyType == typeof(byte))
                            {
                                setvalue = EndianValue[0];
                            }

                            if (property.PropertyType == typeof(byte[]))
                            {
                                //do nothing
                                setvalue = EndianValue.ToArray();
                            }

                            if (property.PropertyType == typeof(string))
                            {

                                //setvalue = Encoding.ASCII.GetString(EndianValue.ToArray());
                                setvalue = Encoding.UTF8.GetString(EndianValue.ToArray());
                            }

                            if (property.PropertyType == typeof(Int16))
                            {

                                setvalue = EndianValue.ToArray().GetInt16(0);
                            }
                            if (property.PropertyType == typeof(UInt16))
                            {

                                setvalue = EndianValue.ToArray().GetUInt16(0);
                            }

                            if (property.PropertyType == typeof(Int32))
                            {
                                setvalue = EndianValue.ToArray().GetInt32(0);
                            }
                            if (property.PropertyType == typeof(UInt32))
                            {

                                setvalue = EndianValue.ToArray().GetUInt32(0);
                            }

                            if (property.PropertyType == typeof(Int64))
                            {
                                setvalue = EndianValue.ToArray().GetInt64(0);
                            }



                            if (property.PropertyType == typeof(DateTime))
                            {

                                if (IsLittleEndian)
                                {
                                    var startIndex = 0;
                                    var rtc = bytes.GetInt32(startIndex);
                                    var offset = bytes.GetInt32(startIndex + 4);

                                    setvalue = DateTimeExtension.GetDateTime(rtc, offset);

                                }
                                else
                                {
                                    var startIndex = 0;
                                    var rtc = bytes.GetInt32BigE(startIndex);
                                    var offset = bytes.GetInt32BigE(startIndex + 4);

                                    setvalue = DateTimeExtension.GetDateTime(rtc, offset);
                                }

                            }

                            if (property.PropertyType.GetTypeInfo().IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                var binaryElementLists = (IEnumerable<BinaryElementList>)property.GetCustomAttributes(typeof(BinaryElementList));
                                if (binaryElementLists.Count() > 0)
                                {
                                    BinaryElementList list = binaryElementLists.First();
                                    //var listElement=Activator.CreateInstance(list.Type);
                                    var listvalue = (IList)property.GetValue(byteClass);
                                    byte listCount = (byte)serInfo.InfoElements[list.CountProperty].Info.GetValue(byteClass);

                                    int listlength = listCount * list.ByteSize;

                                    //var listbytes = value.ToList().GetRange(current.Element.FieldOffset, listlength).ToList();

                                    var listbytes = value;
                                    List<Byte[]> splitBytes = new List<byte[]>();
                                    for (int j = 0; j < listCount; j++)
                                    {
                                        int startindex = j * list.ByteSize;

                                        var serByte = value.ToList().GetRange(startindex, list.ByteSize);

                                        var serializerMethodes = typeof(CGM.Communication.Common.Serialize.Serializer).GetRuntimeMethods();
                                        var generic0 = serializerMethodes.FirstOrDefault(e => e.Name == "DeserializeInternal");
                                        var generic = generic0.MakeGenericMethod(list.Type);
                                        var elementValue = generic.Invoke(this, new object[] { serByte.ToArray() });
                                        listvalue.Add(elementValue);

                                    }


                                    setvalue = listvalue;
                                }
                            }

                            var attributes = (IEnumerable<MessageType>)property.GetCustomAttributes(typeof(MessageType));

                            bool containssubtype = property.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryType));
                            if (containssubtype || attributes.Count() > 0)
                            {
                                Type SeriType = null;
                                int lengthEquals = 0;
                                if (containssubtype)
                                {
                                    SeriType = property.PropertyType;
                                }



                                if (attributes != null && attributes.Count() > 0)
                                {
                                    foreach (var item in attributes)
                                    {
                                        lengthEquals = item.LengthEquals;
                                        if (!string.IsNullOrEmpty(item.Path))
                                        {
                                            var prop = serInfo.InfoElements.FirstOrDefault(e => e.Value.Info.Name == item.Path);

                                            if (item.Value is IEnumerable<byte>)
                                            {
                                                var newValue = (IEnumerable<byte>)item.Value;
                                                var validationValue = bytes.ToList().GetRange(prop.Value.Element.FieldOffset, newValue.Count());
                                                if (validationValue.SequenceEqual(newValue))
                                                {
                                                    SeriType = item.Type;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                var propinfo = serInfo.PropertyInfos.FirstOrDefault(e => e.Name == item.Path);
                                                var validationValue = propinfo.GetValue(byteClass);
                                                if (validationValue.Equals(item.Value))
                                                {
                                                    SeriType = item.Type;
                                                    break;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (item.ReportPattern != null)
                                            {
                                                if (item.ReportPattern.Evaluate(_byteLevels[0]))
                                                {
                                                    SeriType = item.Type;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                SeriType = item.Type;
                                                break;
                                            }

                                        }


                                    }
                                }
                                if (SeriType != null)
                                {
                                    //if (lengthEquals != 0 && lengthEquals != value.Count)
                                    //{
                                    //    throw new Exception($"Expected length {lengthEquals}. Is {value.Count}");
                                    //}
                                    var serializerMethodes = typeof(CGM.Communication.Common.Serialize.Serializer).GetRuntimeMethods();
                                    var generic0 = serializerMethodes.FirstOrDefault(e => e.Name == "DeserializeInternal");
                                    var generic = generic0.MakeGenericMethod(SeriType);
                                    setvalue = generic.Invoke(this, new object[] { value.ToArray() });
                                    var ctors = (IEnumerable<BinaryPropertyValueTransfer>)property.GetCustomAttributes(typeof(BinaryPropertyValueTransfer));
                                    if (ctors.Count() > 0)
                                    {
                                        foreach (var item in ctors)
                                        {
                                            var propinfo = serInfo.PropertyInfos.FirstOrDefault(e => e.Name == item.ParentPropertyName);
                                            var parentvalue = propinfo.GetValue(byteClass);

                                            setvalue.GetType().GetProperty(item.ChildPropertyName).SetValue(setvalue, parentvalue);
                                        }

                                    }
                                    if (property.PropertyType == typeof(SgDataType))
                                    {
                                        ((SgDataType)setvalue).BGUnits = this._session.PumpSettings.DeviceCharacteristics.BgUnitRaw;
                                    }
                                    if (property.PropertyType == typeof(BgDataType))
                                    {
                                        ((BgDataType)setvalue).BGUnits = this._session.PumpSettings.DeviceCharacteristics.BgUnitRaw;
                                    }
                                }

                            }





                            // bool containssubtype = property.PropertyType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryType));



                            property.SetValue(byteClass, setvalue);
                        }
                    }
                }
                if (typeof(T).GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBinaryDeserializationSetting)))
                {
                    ((IBinaryDeserializationSetting)byteClass).OnDeserialization(bytes, _session);
                }
                return byteClass;
            }


            return null;
        }

    }



    public class TypeSwitch
    {
        Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();
        public TypeSwitch Case<T>(Action<T> action) { matches.Add(typeof(T), (x) => action((T)x)); return this; }
        public void Switch(object x) { matches[x.GetType()](x); }
    }
}
