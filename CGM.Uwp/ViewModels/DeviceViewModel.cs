using CGM.Communication.Common.Serialize;
using CGM.Communication.Data;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Responses;
using CGM.Uwp.Helpers;
using CGM.Uwp.Models;
using CGM.Uwp.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace CGM.Uwp.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        ILogger Logger = ApplicationLogging.CreateLogger<DeviceViewModel>();
        private Nightscout _task = new Nightscout();
        private bool _isStarted;
        private SerializerSession _session;
        private PumpStatusMessage _message;

        public PumpStatusMessage CurrentMessage
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        private string _currentTime;

        public string CurrentTime
        {
            get { return _currentTime; }
            set { Set(ref _currentTime, value); }
        }

        private bool _isconnected;

        public bool IsConnected
        {
            get { return _isconnected; }
            set
            {
                Set(ref _isconnected, value);

            }
        }

        private string _nextRun;

        public string NextRun
        {
            get { return _nextRun; }
            set { Set(ref _nextRun, value); }
        }

        private DateTime? _nextRunDateTime;

        public DateTime? NextRunDateTime
        {
            get { return _nextRunDateTime; }
            set { Set(ref _nextRunDateTime, value); }
        }

        private bool _on;
        public bool On
        {
            get { return _on; }
            set
            {

                Set(ref _on, value);

            }
        }

        private string _pumpTime;

        public string PumpTime
        {
            get { return _pumpTime; }
            set { Set(ref _pumpTime, value); }
        }

        private string _alert;

        public string Alert
        {
            get { return _alert; }
            set { Set(ref _alert, value); }
        }

        private bool _showAlert;

        public bool ShowAlert
        {
            get { return _showAlert; }
            set { Set(ref _showAlert, value); }
        }

        public FixedSizeObservableCollection<PumpStatusMessage> PumpStatusMessages { get { return ((App)App.Current).Data.PumpStatusMessages; } }

        public DeviceViewModel()
        {

            PumpStatusMessages.CollectionChanged += PumpStatusMessages_CollectionChanged;
            Messenger.Default.Register<SerializerSession>(this, (session) => UpdatedSession(session));
            Messenger.Default.Register<BayerUsbDevice>(this, (device) => StatusChanged(device));
            this.PropertyChanged += DeviceViewModel_PropertyChanged;


            if (((App)App.Current).Device != null)
            {
                this.IsConnected = ((App)App.Current).Device.IsConnected;
                this.On = this.IsConnected;
            }



            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Start();

        }

        private void DeviceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(On))
            {
                AutoStart();
            }
        }

        private void AutoStart()
        {
            if (On)
            {
                //should run
                //Setting settings;
                //using (CGM.Communication.Data.Repository.CgmUnitOfWork uow =new Communication.Data.Repository.CgmUnitOfWork())
                //{
                //   settings= uow.Setting.GetSettings();
                //}
                if (!_isStarted && IsConnected)
                {
                   StartExtended();
                    //Start();
                }

            }
            else
            {
                if (_isStarted)
                {
                    Stop();
                }
            }

        }

        private async void UpdatedSession(SerializerSession session)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    if (session != null)
    {
        _session = session;
        this.NextRunDateTime = session.NextRun;
        if (session.PumpTime != null && session.PumpTime.PumpDateTime.HasValue)
        {
            var tDif = session.PumpTime.PumpDateTime.Value.Subtract(DateTime.Now);
            string difference = string.Format("{0}:{1}:{2}", tDif.Hours, tDif.Minutes, tDif.Seconds);


            this.PumpTime = string.Format("{0} ({1})", session.PumpTime.PumpDateTime.Value.ToString("HH:mm"), difference);
        }
    }
});
        }

        private async void PumpStatusMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
     () =>
     {
         if (e.NewItems != null && e.NewItems.Count > 0)
         {
             CurrentMessage = (PumpStatusMessage)e.NewItems[0];
             if (CurrentMessage.AlertName != Communication.MiniMed.Model.Alerts.No_Warning_0)
             {
                 this.Alert = CurrentMessage.AlertName.ToString();
                 this.ShowAlert = true;
             }
             else
             {
                 this.Alert = "";
                 this.ShowAlert = false;
             }
         }
         else
         {
             CurrentMessage = null;
         }

     });
        }

        private async void NewStatus(PumpStatusMessage entry)
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.CurrentMessage = entry;
                    });


        }

        public async void StatusChanged(BayerUsbDevice device)
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.IsConnected = device.IsConnected;
                        Setting settings;
                        using (CGM.Communication.Data.Repository.CgmUnitOfWork uow = new Communication.Data.Repository.CgmUnitOfWork())
                        {
                            settings = uow.Setting.GetSettings();
                            if (this.IsConnected && settings.OtherSettings.AutoStartTask)
                            {
                                this.On = true;
                            }
                            else
                            {
                                this.On = false;
                            }
                        }
                        

                    });


        }

        void timer_Tick(object sender, object e)
        {
            DateTime dt = DateTime.Now;
            if (_session != null && NextRunDateTime.HasValue)
            {
                var tm = NextRunDateTime.Value.Subtract(dt);
                NextRun = string.Format("{0:D2}:{1:D2}", tm.Minutes, tm.Seconds);
            }

            CurrentTime = dt.ToString("HH:mm");
        }

        public void Stop()
        {
            Logger.LogInformation("Task stopped");
            _task.Stop();
            NextRun = "";
            NextRunDateTime = null;
            this._isStarted = false;
        }


        public void Start()
        {
            Logger.LogInformation("Task started");

            //testdata
            //((App)App.Current).Data.CreateTestData();

            _task.Start(((App)App.Current).Device);
            _isStarted = true;
        }

        ExtendedExecutionSession session = null;

        public async Task StartExtended()
        {
            ClearExtendedExecution();

            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.LocationTracking;
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    session = newSession;
                    Start();
                    break;

                default:
                case ExtendedExecutionResult.Denied:
                    newSession.Dispose();
                    break;
            }

        }
        private void EndExtendedExecution()
        {
            ClearExtendedExecution();
        }

        void ClearExtendedExecution()
        {
            if (session != null)
            {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }

        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                Logger.LogWarning($"Session Revoked: {args.Reason}");
                EndExtendedExecution();
            });
        }
    }
}
