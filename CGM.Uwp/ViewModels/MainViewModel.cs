using System;

using GalaSoft.MvvmLight;
using Windows.Devices.HumanInterfaceDevice;
using System.Collections.Generic;
using CGM.Uwp.Models;
using Microsoft.Extensions.Logging;
using CGM.Communication.Log;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using CGM.Uwp.Log;
using GalaSoft.MvvmLight.Messaging;
using CGM.Uwp.Helpers;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Tasks;
using CGM.Uwp.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace CGM.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public FixedSizeObservableCollection<LogEntry> Logs { get { return ((App)App.Current).Data.Logs; } }

        public ICommand ClearLogCommand { get; set; }

        public MainViewModel()
        {
            this.ClearLogCommand   = new RelayCommand(() => Logs.Clear());
        }

    }
}
