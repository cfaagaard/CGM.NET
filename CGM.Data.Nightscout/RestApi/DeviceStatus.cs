using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Nightscout.RestApi
{
    public class DeviceStatus : System.ComponentModel.INotifyPropertyChanged
    {


        private double _uploaderBattery;
        private string _device;
        private string _createdAt;

        [Newtonsoft.Json.JsonProperty("device", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [BsonElement("device")]
        public string Device
        {
            get { return _device; }
            set
            {
                if (_device != value)
                {
                    _device = value;
                    RaisePropertyChanged();
                }
            }
        }



        private PumpInfo _pump;
        [BsonElement("pump")]
        [Newtonsoft.Json.JsonProperty("pump", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PumpInfo PumpInfo
        {
            get { return _pump; }
            set
            {
                if (_pump != value)
                {
                    _pump = value;
                    RaisePropertyChanged();
                }
            }
        }

        [BsonElement("created_at")]
        [Newtonsoft.Json.JsonProperty("created_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    RaisePropertyChanged();
                }
            }
        }
        [BsonElement("uploaderBattery")]
        [Newtonsoft.Json.JsonProperty("uploaderBattery", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double UploaderBattery
        {
            get { return _uploaderBattery; }
            set
            {
                if (_uploaderBattery != value)
                {
                    _uploaderBattery = value;
                    RaisePropertyChanged();
                }
            }
        }


        public DeviceStatus()
        {
            this.PumpInfo = new PumpInfo();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static DeviceStatus FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceStatus>(data);
        }

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        //        {
        //    "_id": {
        //        "$oid": "591794a6642a16c88a484673"
        //    },
        //    "device": "medtronic-640g://6213-1002345",
        //    "pump": {
        //        "reservoir": 109.675,
        //        "iob": {
        //            "timestamp": "Sun May 14 01:20:04 CEST 2017",
        //            "bolusiob": 1.2000000476837158
        //        },
        //        "status": {
        //            "suspended": true
        //        },
        //        "clock": "2017-05-14T01:20:04+0200",
        //        "battery": {
        //            "percent": 75
        //        }
        //    },
        //    "created_at": "2017-05-14T01:20:04+0200",
        //    "uploaderBattery": 57
        //}

        //uploaderBattery
        //    device
        //    created_at

        //    pump:
        //    clock
        //    reservoir

        //    iob:
        //    timestamp
        //    bolusiob



        //    status:
        //    bolusing:true
        //    suspended:true
        //    status=normal

        //    battery:
        //    percent
    }

    public class PumpInfo : System.ComponentModel.INotifyPropertyChanged
    {

        private double _reservoir;
        [BsonElement("reservoir")]
        [Newtonsoft.Json.JsonProperty("reservoir", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Reservoir
        {
            get { return _reservoir; }
            set
            {
                if (_reservoir != value)
                {
                    _reservoir = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Iob _iob;
        [BsonElement("iob")]
        [Newtonsoft.Json.JsonProperty("iob", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Iob Iob
        {
            get { return _iob; }
            set
            {
                if (_iob != value)
                {
                    _iob = value;
                    RaisePropertyChanged();
                }
            }
        }




        private Dictionary<string, object> _status;
        [BsonElement("status")]
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Dictionary<string,object> Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _clock;
        [BsonElement("clock")]
        [Newtonsoft.Json.JsonProperty("clock", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Clock
        {
            get { return _clock; }
            set
            {
                if (_clock != value)
                {
                    _clock = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Battery _battery;
        [BsonElement("battery")]
        [Newtonsoft.Json.JsonProperty("battery", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Battery Battery
        {
            get { return _battery; }
            set
            {
                if (_battery != value)
                {
                    _battery = value;
                    RaisePropertyChanged();
                }
            }
        }

        public PumpInfo()
        {
            Status = new Dictionary<string, object>();
            Battery = new Battery();
            Iob = new Iob();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static PumpInfo FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PumpInfo>(data);
        }

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }




    public class Iob : System.ComponentModel.INotifyPropertyChanged
    {

        private string _timestamp;
        [BsonElement("timestamp")]
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _bolusiob;
        [BsonElement("bolusiob")]
        [Newtonsoft.Json.JsonProperty("bolusiob", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Bolusiob
        {
            get { return _bolusiob; }
            set
            {
                if (_bolusiob != value)
                {
                    _bolusiob = value;
                    RaisePropertyChanged();
                }
            }
        }



        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static PumpInfo FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PumpInfo>(data);
        }

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }

    public class Battery : System.ComponentModel.INotifyPropertyChanged
    {



        private int _percent;
        [BsonElement("percent")]
        [Newtonsoft.Json.JsonProperty("percent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Percent
        {
            get { return _percent; }
            set
            {
                if (_percent != value)
                {
                    _percent = value;
                    RaisePropertyChanged();
                }
            }
        }



        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static PumpInfo FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PumpInfo>(data);
        }

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
