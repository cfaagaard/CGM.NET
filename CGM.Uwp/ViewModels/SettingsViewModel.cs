using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CGM.Uwp.Services;
using Windows.ApplicationModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;
using CGM.Communication.Common;
using CMG.Data.Sqlite.Repository;
using CMG.Data.Sqlite;

namespace CGM.Uwp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

        private bool _isLightThemeEnabled;
        private IDialogService _dialogService;
        public bool IsLightThemeEnabled
        {
            get { return _isLightThemeEnabled; }
            set { Set(ref _isLightThemeEnabled, value); }
        }

        private string _appDescription;
        public string AppDescription
        {
            get { return _appDescription; }
            set { Set(ref _appDescription, value); }
        }

        private Configuration _setting;

        public string NightscoutUrl
        {
            get { return _setting.NightscoutUrl; }
            set
            {
                _setting.NightscoutUrl = value;
                SaveSetting();
            }
        }

        public string NotificationUrl
        {
            get { return _setting.NotificationUrl; }
            set
            {
                _setting.NotificationUrl = value;
                SaveSetting();
            }
        }

        public string NightscoutApiKey
        {
            get { return _setting.NightscoutSecretkey; }
            set
            {
                _setting.NightscoutSecretkey = value;
                SaveSetting();
                this.RaisePropertyChanged(nameof(NightscoutApiKey));
                this.RaisePropertyChanged(nameof(NightscoutApiKeyHashed));
            }
        }

        private string _localpath;

        public string LocalPath
        {
            get { return _localpath; }
            set { Set(ref _localpath, value); }
        }

        private DeviceItemViewModel _selectedDevice;

        public DeviceItemViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set { Set(

                ref _selectedDevice, value);
            }
        }

       
        public bool  IncludeHistory
        {
            get { return _setting.IncludeHistory; }
            set
            {
                _setting.IncludeHistory = value;
                RaisePropertyChanged(nameof(IncludeHistory));
                SaveSetting();
            }
        }

        public bool SendEventsToNotificationUrl
        {
            get { return _setting.SendEventsToNotificationUrl; }
            set
            {
                _setting.SendEventsToNotificationUrl = value;
                this.RaisePropertyChanged(nameof(SendEventsToNotificationUrl));
                SaveSetting();
            }
        }

        public int HistoryDaysBack
        {
            get { return _setting.HistoryDaysBack; }
            set
            {
                _setting.HistoryDaysBack = value;
                this.RaisePropertyChanged(nameof(HistoryDaysBack));
                SaveSetting();
            }
        }

        public int IntervalSeconds
        {
            get { return _setting.IntervalSeconds; }
            set
            {
                _setting.IntervalSeconds = value;
                this.RaisePropertyChanged(nameof(IntervalSeconds));
                SaveSetting();
            }
        }
        public int TimeoutSeconds
        {
            get { return _setting.TimeoutSeconds; }
            set
            {
                _setting.TimeoutSeconds = value;
                this.RaisePropertyChanged(nameof(TimeoutSeconds));
                SaveSetting();
            }
        }
        

        public bool AutoStartTask
        {
            get { return _setting.AutoStartTask; }
            set
            {
                _setting.AutoStartTask = value;
                this.RaisePropertyChanged(nameof(AutoStartTask));
                SaveSetting();
            }
        }

        public bool OnlyFromTheLastReading
        {
            get { return _setting.OnlyFromTheLastReading; }
            set
            {
                _setting.OnlyFromTheLastReading = value;
                this.RaisePropertyChanged(nameof(OnlyFromTheLastReading));
                SaveSetting();
            }
        }

        public bool UploadToNightscout
        {
            get { return _setting.UploadToNightscout; }
            set
            {
                _setting.UploadToNightscout = value;
                this.RaisePropertyChanged(nameof(UploadToNightscout));
                SaveSetting();
            }
        }

        public bool MongoUpload
        {
            get { return _setting.MongoUpload; }
            set
            {
                _setting.MongoUpload = value;
                this.RaisePropertyChanged(nameof(MongoUpload));
                SaveSetting();
            }
        }

        public string MongoDbUrl
        {
            get { return _setting.MongoDbUrl; }
            set
            {
                _setting.MongoDbUrl = value;
                SaveSetting();
                this.RaisePropertyChanged(nameof(MongoDbUrl));
            }
        }

        public bool HandleAlert776
        {
            get { return _setting.HandleAlert776; }
            set
            {
                _setting.HandleAlert776 = value;
                this.RaisePropertyChanged(nameof(HandleAlert776));
                SaveSetting();
            }
        }

        public ObservableCollection<DeviceItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceItemViewModel>();



        public string NightscoutApiKeyHashed { get { return _setting.ApiKeyHashed; } }


        public ICommand SwitchThemeCommand { get; private set; }
        public ICommand GetDevicesCommand { get; private set; }

        public ICommand ClearHistoryCommand { get; private set; }

        public SettingsViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SwitchThemeCommand = new RelayCommand(async () => { await ThemeSelectorService.SwitchThemeAsync(); });
            this.GetDevicesCommand = new RelayCommand(() => this.GetDevices());

            this.ClearHistoryCommand = new RelayCommand(() => this.ClearHistory());
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                _setting = uow.Setting.GetSettings();
            }
            GetDevices();
            this.LocalPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }

        private void ClearHistory()
        {
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                uow.History.ResetHistory();
            }
            _dialogService.ShowMessage("History has been cleared.","Clear history");
        }

        private void GetDevices()
        {
            this.Devices.Clear();
            List<Device> devices=new List<Device>();
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                devices = uow.Device.GetAllDevices();
            }

            foreach (var item in devices)
            {
                var device = new DeviceItemViewModel(item, _dialogService);
                device.Removed += Device_Removed;
                this.Devices.Add(device);
            }
            if (this.Devices.Count>0)
            {
                this.SelectedDevice = this.Devices[0];
            }
            else
            {
                this.SelectedDevice = null;
            }
        }

        private void Device_Removed(object sender, EventArgs e)
        {
            GetDevices();
        }

        private void SaveSetting()
        {
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                uow.Setting.Update(_setting);
            }
        }

        public void Initialize()
        {
            IsLightThemeEnabled = ThemeSelectorService.IsLightThemeEnabled;
            AppDescription = GetAppDescription();
        }

        private string GetAppDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }


    }
}
