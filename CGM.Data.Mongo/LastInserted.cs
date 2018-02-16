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
    }

    public class LastInsertedEvent
    {
        public int Rtc { get; set; } = 0;
    }
}
