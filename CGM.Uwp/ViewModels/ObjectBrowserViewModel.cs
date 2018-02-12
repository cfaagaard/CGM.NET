//using CGM.Communication.Common.Serialize;
//using CGM.Communication.Data.Repository;
//using CGM.Communication.MiniMed.Infrastructur;
//using CGM.Communication.MiniMed.Responses.Events;
//using CGM.Uwp.Models;
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
//using GalaSoft.MvvmLight.Messaging;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Windows.ApplicationModel.Core;
//using Windows.UI.Core;

//namespace CGM.Uwp.ViewModels
//{
//    public class ObjectBrowserViewModel : ViewModelBase
//    {
        

//        public ICommand GetSessionCommand { get; set; }

//        private SerializerSession _session;

//        public SerializerSession Session
//        {
//            get { return _session; }
//            set { Set(ref _session, value); }
//        }

//        public BayerUsbDevice Device
//        {
//            get { return ((App)App.Current).Device; }
//        }


//        private ObservableCollection<HistoryDataTypeEnum> _historyTypes = new ObservableCollection<HistoryDataTypeEnum>();

//        public ObservableCollection<HistoryDataTypeEnum> HistoryTypes
//        {
//            get { return _historyTypes; }
//            set { Set(ref _historyTypes, value); }

//        }

//        private HistoryDataTypeEnum? _selectedHistoryType;

//        public HistoryDataTypeEnum? SelectedHistoryType
//        {
//            get { return _selectedHistoryType; }
//            set {
//                Set(ref _selectedHistoryType, value);
//                SetEvents();
//            }
//        }

//        private ObservableCollection<PumpEvent> _events = new ObservableCollection<PumpEvent>();

//        public ObservableCollection<PumpEvent> Events
//        {
//            get { return _events; }
//            set { Set(ref _events, value); }

//        }

//        public ObjectBrowserViewModel()
//        {
//             Messenger.Default.Register<SerializerSession>(this, (session) => UpdatedSession(session));
//            //Messenger.Default.Register<BayerUsbDevice>(this, (device) => StatusChanged(device));
//            this.GetSessionCommand = new RelayCommand(() =>Task.Run(()=> GetSession()).Wait());
//        }

//        private void SetEvents()
//        {
//            Events.Clear();
//            if (this.SelectedHistoryType!=null)
//            {
//                var handler = Session.PumpDataHistory.MultiPacketHandlers.FirstOrDefault(e => e.ReadInfoResponse.HistoryDataType == this.SelectedHistoryType);
//                var events = handler.JoinAllEvents();
//                events.ForEach(e => Events.Add(e));
//            }

            
//        }

//        //public async void StatusChanged(BayerUsbDevice device)
//        //{

//        //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
//        //            () =>
//        //            {
//        //                this.Device = device;
                        
//        //            });
//        //}

//        private async Task GetSession()
//        {
//            SerializerSession session = null;
//            try
//            {

//               var _tokenSource = new CancellationTokenSource();
//               var _token = _tokenSource.Token;
//                using (CgmUnitOfWork uow = new CgmUnitOfWork())
//                {

//                    session = await uow.Pump.GetPumpSessionAsync(this.Device,_token);
//                    if (session != null)
//                    {
//                        UpdateSession(session);
//                    }
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        private void UpdateSession(SerializerSession session)
//        {
//            this.Session = session;
//            HistoryTypes.Clear();
//            foreach (var item in session.PumpDataHistory.MultiPacketHandlers)
//            {
//                HistoryTypes.Add(item.ReadInfoResponse.HistoryDataType);
//            }
//            if (this.HistoryTypes.Count>0)
//            {
//                this.SelectedHistoryType = HistoryTypes.FirstOrDefault();
//            }
            
            
//        }

//        private async void UpdatedSession(SerializerSession session)
//        {
//            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
//                () =>
//                {
//                    if (session != null)
//                    {
//                        UpdateSession(session);
//                    }
//                });
//        }
//    }
//}


