using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Model
{
    public class History
    {
        //key is a combined by "historyDataType", "eventtype" and "rtcBytes". Should be ok.
        //key is the byte-string without offset....

        public string Key { get; set; }

        public int EventType { get; set; }

        public int Rtc { get; set; }

        public int HistoryDataType { get; set; }

       
        public string HistoryBytes { get; set; }



        public History()
        {

        }
        //int historyDataType, int eventType, int rtc, string historyBytes
        public History(BasePumpEvent pumpEvent)
        {
            //remove offset from bytestring.... offset can change when setting time on pump (changing timezone)
            this.Key = pumpEvent.Key;
            //this.Key =  pumpEvent.Key;
            this.HistoryDataType = pumpEvent.HistoryDataType;
            this.Rtc = pumpEvent.EventDate.Rtc;
            this.EventType = (int)pumpEvent.EventType;
            this.HistoryBytes = pumpEvent.BytesAsString;
        }

        public override string ToString()
        {
            return this.Key;
        }
    }

}
