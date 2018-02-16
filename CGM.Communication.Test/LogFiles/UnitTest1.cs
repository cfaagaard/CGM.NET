using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using System.Linq;
using CGM.Communication.MiniMed.Responses.Events;
using System.Collections.Generic;
using System.Data;

namespace CGM.Communication.Test.LogFiles
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadLogFile()
        {

            SerializerSession session = new SerializerSession();

            //this will read the log file
            //the enclose logfile only holds data for one read. But you can have a logfile with several reads.
            //it has history 2 days back in the read.
            //the events has also a sensor change in it
            LogFileReader file = new LogFileReader(@"LogFiles\Files\single1.txt", session);

            //the session class will now hold the information about the session.
            //Look at the collection "status" for the statusmessage.

            var readings = session.PumpDataHistory.SensorEvents.Where(e => e.EventType == MiniMed.Infrastructur.EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED).Select(e => (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)e.Message).ToList();

            List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> details = new List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>();

            readings.ForEach(e => details.AddRange(e.Details));

            DataTable dt = new DataTable();
            dt.Columns.Add("ISIG");
            dt.Columns.Add("Sgv");
            dt.Columns.Add("RateOfChange");

            var isig = details.Select(e => new detail() { ISIG = e.IsigRaw, Sgv = e.Amount, Rateofchange = e.RateOfChangeRaw }).ToList();


            foreach (var item in isig)
            {
                DataRow row = dt.NewRow();
                row[0] = item.ISIG;
                row[1] = item.Sgv;
                row[2] = item.Rateofchange;
                dt.Rows.Add(row);
            }

      


        }

        class detail
        {
            public short ISIG { get; set; }
            public int Sgv { get; set; }

            public double Rateofchange { get; set; }
        }
    }
}
