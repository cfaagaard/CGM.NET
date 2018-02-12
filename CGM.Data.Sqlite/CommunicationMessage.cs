using System;
using System.Collections.Generic;
using System.Text;

namespace  CMG.Data.Sqlite
{
    public class CommunicationMessage
    {
        [SQLite.PrimaryKey]
        public string MessageKey { get; set; }  

        public DateTime MessageDateTime { get; set; }

        [SQLite.MaxLength(200)]
        public string MessageType { get; set; }

        [SQLite.MaxLength(200)]
        public string MessageSubType { get; set; }

        [SQLite.MaxLength(300)]
        public string Message { get; set; }

        public int NightScoutStatus { get; set; } = 0;
        //0 should not be handle
        //1 waiting to be handled (uploaded)
        //2 has been handled


        public int NotificationStatus { get; set; } = 0;
        //0 do not notify
        //1 should be send
        //2 has been sent

        public override string ToString()
        {
            return this.MessageKey;
        }
    }
}
