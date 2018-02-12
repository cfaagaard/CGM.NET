using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace  CMG.Data.Sqlite
{
    public class History
    {
        //key is a combined by "historyDataType", "eventtype" and "rtcBytes". Should be ok. no it is not.....
        //key is the byte-string without offset....
        [SQLite.PrimaryKey, SQLite.MaxLength(500)]
        public string Key { get; set; }

        public int EventType { get; set; }

        public int Rtc { get; set; }

        public int HistoryDataType { get; set; }

        [SQLite.MaxLength(3000)]
        public string HistoryBytes { get; set; }



        public History()
        {

        }
        //int historyDataType, int eventType, int rtc, string historyBytes
        public History(PumpEvent pumpEvent)
        {
            //remove offset from bytestring.... offset can change when setting time on pump (changing timezone)
            this.Key = pumpEvent.Key;
            //this.Key =  pumpEvent.Key;
            this.HistoryDataType = pumpEvent.HistoryDataType;
            this.Rtc = pumpEvent.Rtc;
            this.EventType = (int)pumpEvent.EventType;
            this.HistoryBytes = pumpEvent.BytesAsString;
        }

        public override string ToString()
        {
            return this.Key;
        }
    }

}
