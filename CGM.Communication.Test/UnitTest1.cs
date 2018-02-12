using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGM.Communication.Test.USB;
using System.Threading.Tasks;
using System.Threading;
using CGM.Communication.Test.LogToOutput;

namespace CGM.Communication.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TimeTest()
        {

        }

        [TestMethod]
        public void GetCnlInformation()
        {
            Task.Run(() => GetSession()).Wait();
        }

        private async Task GetSession()
        {
            //with this, the log is coming in the output window when we debug.
            Log.ApplicationLogging.LoggerFactory.AddOutputLogger();

            //find the device
            var device = BayerUsbDevice.FindMeter();

            //getting a cancellation token for the tasks
            var _tokenSource = new CancellationTokenSource();
            var _token = _tokenSource.Token;




            //unit of work to get information
            //using (CgmUnitOfWork uow = new CgmUnitOfWork())
            //{
            //    //Check sqlite database version, should be done at the begin of every run to make sure database is aval and correct version.
            //    uow.CheckDatabaseVersion(AppContext.BaseDirectory);

            //    //Getting info from pump. The session class contain all info necessary for communicating with the pump and all the results. 
            //    //Common.Serialize.SerializerSession session = await uow.Pump.GetPumpSessionAsync(device, _token);

            //}
        }
    }
}
