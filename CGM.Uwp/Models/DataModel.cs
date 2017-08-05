using CGM.Communication.MiniMed.Responses;
using CGM.Uwp.Helpers;
using CGM.Uwp.Log;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using CGM.Communication.Extensions;
using Microsoft.Extensions.Logging;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Model;
using CGM.Communication.Common.Serialize;

namespace CGM.Uwp.Models
{
    public class DataModel
    {
        private ILogger Logger = ApplicationLogging.CreateLogger<DataModel>();
        public FixedSizeObservableCollection<LogEntry> Logs { get; private set; } = new FixedSizeObservableCollection<LogEntry>(100, true);
        public FixedSizeObservableCollection<PumpStatusMessage> PumpStatusMessages { get; private set; } = new FixedSizeObservableCollection<PumpStatusMessage>(30, true);


        public DataModel()
        {
            Messenger.Default.Register<LogEntry>(this, (entry) => LogToCollection(entry));
            Messenger.Default.Register<SerializerSession>(this, (session) => UpdatedSession(session));

        }



        public async void LogToCollection(LogEntry entry)
        {
            if (entry.LogLevel == LogLevel.Information || entry.LogLevel == LogLevel.Error)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this.Logs.Insert(0, entry);
            });
            }
        }


        private async void UpdatedSession(SerializerSession session)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
      () =>
      {
          if (session.Status.Count > 0)
          {
              //var entry = session.Status.Last();
              //PumpStatusMessages.Insert(0, entry);
              foreach (var item in session.Status)
              {
                  PumpStatusMessages.Insert(0, item);
              }
          }

      });
        }

        public void CreateTestData()
        {

            SerializerSession session = new SerializerSession();

            for (int i = 0; i < 30; i++)
            {
                session.Status.Add(GetRandomMessage(i));
            }

            var last=session.Status.Last();

            int offsetInt = -1665586902;

            var offset = BitConverter.GetBytes(offsetInt);

            session.PumpTime.Rtc =  DateTime.Now.AddSeconds(-55).GetRtcBytes(offsetInt).Reverse().ToArray();
            session.PumpTime.OffSet = offset;

            Messenger.Default.Send<SerializerSession>(session);

        }



        Random r = new Random();
        Random random = new Random();
        Random randomw = new Random();
        //for testing
        public PumpStatusMessage GetRandomMessage(int number)
        {

            List<byte> randomtrend = new List<byte>() { 0x60, 0xc0, 0xa0, 0x80, 0x40, 0x20, 0x00 };
            // List<short> randomWarnings = new List<short>() { 0,0,0,0,806,810,869,775,816,777,780,781};
            var warningsvalues = Enum.GetValues(typeof(Alerts)).Cast<Alerts>();
            List<Alerts> warns = new List<Alerts>();
            foreach (var item in warningsvalues)
            {
                warns.Add((Alerts)item);
            }

            PumpStatusMessage msg = new PumpStatusMessage();
            msg.Sgv = (short)r.Next(80, 400);
            msg.SgvDateTime = new Communication.MiniMed.DataTypes.DateTimeDataType();

            msg.SgvDateTime.Offset = -1665586902;
            msg.SgvDateTime.Rtc = DateTime.Now.GetRtcBytes(msg.SgvDateTime.Offset).GetInt32BigE(0);// - 2079185194;

            msg.CgmTrend = (byte)randomtrend[random.Next(randomtrend.Count)];

            msg.Alert = (short)warns[randomw.Next(warns.Count)];
            if (msg.Alert != 0)
            {
                msg.AlertDateTime = new Communication.MiniMed.DataTypes.DateTimeDataType(DateTime.Now.GetRtcBytes(-1665586902).GetInt32BigE(0), -1665586902);

            }


            return msg;
        }
    }
}
