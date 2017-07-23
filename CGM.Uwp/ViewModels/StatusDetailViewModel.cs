using System;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using CGM.Uwp.Models;
using CGM.Uwp.Services;

using Windows.UI.Xaml;
using CGM.Communication.MiniMed.Responses;
using System.Collections.Generic;
using System.Reflection;

namespace CGM.Uwp.ViewModels
{

    public class StatusDetailViewModel : ViewModelBase
    {

        public NavigationServiceEx NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        public ICommand StateChangedCommand { get; private set; }

        private PumpStatusMessage _item;
        public PumpStatusMessage Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }



        public StatusDetailViewModel()
        {
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }



        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            if (args.OldState.Name == NarrowStateName && args.NewState.Name == WideStateName)
            {
                NavigationService.GoBack();
            }
        }
    }
}
