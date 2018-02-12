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
              foreach (var item in session.Status)
              {
                  PumpStatusMessages.Insert(0, item);
              }
          }

      });
        }

    }
}
