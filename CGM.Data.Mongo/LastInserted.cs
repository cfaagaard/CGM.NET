using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Mongo
{
   public class LastInserted
    {
        [BsonId]
        public int Key { get; set; } = 1;
        public LastInsertedEvent Pump { get; set; } = new LastInsertedEvent();
        public LastInsertedEvent Sensor { get; set; } = new LastInsertedEvent();
        public string PumpSettings { get; set; }
        public Int32 StatusRtc { get; set; } = 0;
    }

    public class LastInsertedEvent
    {
        public Int32 Rtc { get; set; } = 0;
    }
}
