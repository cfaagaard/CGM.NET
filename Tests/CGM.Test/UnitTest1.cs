using System;
using CGM.Communication.Common.Serialize;
using CGM.Data.Minimed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CGM.Communication.MiniMed.Responses.Events;

namespace CGM.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            string path = @"data/14092018_log_first.txt";
            var session = new SerializerSession();
            session.LoadLog(path);


            var rem = session.PumpDataHistory.PumpEvents.Where(e => e.EventType == Communication.MiniMed.Infrastructur.EventTypeEnum.PLGM_CONTROLLER_STATE).ToList();
            rem.ForEach(e => session.PumpDataHistory.PumpEvents.Remove(e));

            //MapperBootstrapper mapperBootstrapper = new MapperBootstrapper();

            //CGM.Data.Minimed.MiniMedRepository repository = new Data.Minimed.MiniMedRepository();
            ////repository.DeleteAll();
            //repository.OnStartUp();
            //repository.SaveSession(session);


            //var test= (session.PumpDataHistory.PumpEvents[16].Message as BG_READING_Event);
            //  var test2=session.SessionDevice.Device.SerialNumberFull;
        }

        [TestMethod]
        public void Data()
        {
            using (MinimedContext ctx = new MinimedContext())
            {
               var sgReadings= ctx.SensorReadings
                    .Include(e => e.SensorEvent.Pump)
                    .ToList();
            }
        }

        [TestMethod]
        public void logfiles()
        {
            LogFileHistoryAdapter logFileHistoryAdapter = new LogFileHistoryAdapter("data");
            logFileHistoryAdapter.ExtractHistory(new DateTime(2018, 9, 14), new DateTime(2018, 9, 14), true);

        }


        [TestMethod]
        public void TestNightScoutUpload()
        {
            string path = @"data/LogFile.txt";
            var session = new SerializerSession();
            session.LoadLog(path);

            CGM.Data.SessionStateRepository sessionStateRepository = new Data.SessionStateRepository();

            sessionStateRepository.SaveSession(session);

            CGM.Data.Nightscout.RestApi.NightScoutUploader uploader = new Data.Nightscout.RestApi.NightScoutUploader(sessionStateRepository);
            uploader.NightscoutConfiguration.NightscoutUrl = "https://aagaarddiabetes.azurewebsites.net/";
            uploader.NightscoutConfiguration.NightscoutSecretkey = "1234567890abc";

            uploader.ExecuteTask(session,new System.Threading.CancellationToken());


        }
    }
}
