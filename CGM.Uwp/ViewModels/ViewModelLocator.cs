using GalaSoft.MvvmLight.Ioc;

using Microsoft.Practices.ServiceLocation;

using CGM.Uwp.Services;
using CGM.Uwp.Views;
using GalaSoft.MvvmLight.Views;

namespace CGM.Uwp.ViewModels
{
    public class ViewModelLocator
    {
        NavigationServiceEx _navigationService = new NavigationServiceEx();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => _navigationService);
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ShellViewModel>();
            Register<MainViewModel, MainPage>();
            //Register<LogViewModel, LogPage>();
            Register<StatusViewModel, StatusPage>();
            Register<StatusDetailViewModel, StatusDetailPage>();
            Register<SettingsViewModel, SettingsPage>();
            Register<DeviceViewModel, DeviceControl>();
        }

        public DeviceViewModel DeviceViewModel => ServiceLocator.Current.GetInstance<DeviceViewModel>();
        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public StatusDetailViewModel StatusDetailViewModel => ServiceLocator.Current.GetInstance<StatusDetailViewModel>();

        public StatusViewModel StatusViewModel => ServiceLocator.Current.GetInstance<StatusViewModel>();

        //public LogViewModel LogViewModel => ServiceLocator.Current.GetInstance<LogViewModel>();

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public ShellViewModel ShellViewModel => ServiceLocator.Current.GetInstance<ShellViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();
            _navigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
