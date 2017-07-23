using CGM.Communication.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CGM.Uwp.ViewModels
{
    public class DeviceItemViewModel : ViewModelBase
    {
        private IDialogService _dialogService;
        private Device _device;
        public event EventHandler Removed;
        public ICommand DeleteDeviceCommand { get; set; }

        public Device Device { get { return _device; } }

        public Dictionary<string, string> DeviceInformation { get; set; } = new Dictionary<string, string>();

        public DeviceItemViewModel(Device device, IDialogService dialogService)
        {
            _device = device;
            _dialogService = dialogService;
            this.DeleteDeviceCommand = new RelayCommand(() => DeleteDevice());
            CreateInfoDictionary();

        }

        private void CreateInfoDictionary()
        {
            this.DeviceInformation.Add(nameof(Device.Name), Device.Name);
            this.DeviceInformation.Add(nameof(Device.SerialNumber), Device.SerialNumber);
            this.DeviceInformation.Add(nameof(Device.SerialNumberFull), Device.SerialNumberFull);

            this.DeviceInformation.Add(nameof(Device.LinkKey), Device.LinkKey);
            this.DeviceInformation.Add(nameof(Device.LinkMac), Device.LinkMac);
            this.DeviceInformation.Add(nameof(Device.PumpMac), Device.PumpMac);
           
            this.DeviceInformation.Add("LastUsedRadioChannel", Device.RadioChannel);
            //this.DeviceInformation.Add("FullInfo", Device.FullInfo);
        }

        private async void DeleteDevice()
        {

            await _dialogService.ShowMessage($"Are you sure you want to delete device: {this.ToString()}?",
                    "Confirmation",
                    buttonConfirmText: "Ok", buttonCancelText: "Cancel",
                    afterHideCallback: (confirmed) =>
                    {
                        if (confirmed)
                        {
                            using (CGM.Communication.Data.Repository.CgmUnitOfWork uow = new Communication.Data.Repository.CgmUnitOfWork())
                            {
                                uow.Device.Remove(_device);
                            }
                            if (Removed != null)
                            {
                                Removed(this, null);
                            }
                        }
                        else
                        {
                            // User has pressed the "cancel" button
                            // (or has discared the dialog box).
                            // ...
                        }
                    });
        }

        public override string ToString()
        {
            return _device.ToString();
        }
    }
}
