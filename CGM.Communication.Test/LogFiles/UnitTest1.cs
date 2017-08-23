using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;

namespace CGM.Communication.Test.LogFiles
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadLogFile()
        {
            //in order to read log file we need the encryption key (which is unique for each CNL/pump)
            //the encryption key can be found in the log file (for each read)
            // in the enclose file "single1.txt" it is on line 35, show below. It contains different variables used in the communication with the pump
            // the variable you need is: EncryptKey:B4-47-7E-4B-56-D3-D3-9C-F5-A4-AE-49-C1-AD-A6-C8
            //8/22/2017 6:41:14 PM; RadioChannel:14;LinkMac:D7-48-0F-82-06-F7-23-00;PumpMac:7E-65-0F-EE-45-F7-23-00;LinkKey:09-C6-2F-42-90-A6-65-4B-0B-B8-61-57-7E-E8-29-B4-C3-A9-63-D3-E4-07-D3-98-6A-63-C1-AB-F5-5A-A4-0C-3E-AE-6A-B6-89-0D-C1-AE-AD-4A-A6-1A-37-0D-96-AA-B1-56-C9-D4-21-FE-ED;EncryptKey:B4-47-7E-4B-56-D3-D3-9C-F5-A4-AE-49-C1-AD-A6-C8;SerialNumber:1001687;ModelNumber:Bayer6210;SerialNumberFull:6213-1001687;
            //NOTICE: you can NOT use the above variable as this unique for my CNL.
            // you need to change it to your own.
            

            SerializerSession session = new SerializerSession();


            session.EncryptKey = "B4-47-7E-4B-56-D3-D3-9C-F5-A4-AE-49-C1-AD-A6-C8".GetBytes();

            //this will read the log file
            //the enclose logfile only holds data for one read. But you can have a logfile with several reads.
            //it has history 2 days back in the read.
            //the events has also a sensor change in it
            LogFileReader file = new LogFileReader(@"LogFiles\Files\single1.txt", session);

            //the session class will now hold the information about the session.
            //Look at the collection "status" for the statusmessage.

            //if you have used history, then run this (subject to changes)
            session.PumpDataHistory.GetHistoryEvents();
            //this will give all the events
            var allEvents = session.PumpDataHistory.JoinAllEvents();
        }
    }
}
