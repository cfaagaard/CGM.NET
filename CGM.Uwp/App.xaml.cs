using System;

using CGM.Uwp.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using CGM.Communication.Log;
using CGM.Uwp.Log;
using CGM.Uwp.Models;
using CGM.Uwp.Helpers;
using GalaSoft.MvvmLight.Messaging;
using CGM.Uwp.Tasks;
using Microsoft.Extensions.Logging;
using Windows.Foundation.Metadata;

namespace CGM.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public DataModel Data { get; set; }=new DataModel();
        public BayerUsbDevice Device { get; set; }  = new BayerUsbDevice();
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService { get { return _activationService.Value; } }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            //Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(e); 
            }
            string DataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            ApplicationLogging.LoggerFactory.AddCgmLog(DataPath);
            
            ApplicationLogging.LoggerFactory.AddEventAggregatorLog();

            using (Communication.Data.Repository.CgmUnitOfWork uow = new Communication.Data.Repository.CgmUnitOfWork())
            {
                uow.CheckDatabaseVersion(DataPath);
                uow.Setting.CheckSettings();

            }
            //check for nightscout settings




        }


     


        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
            
        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(ViewModels.MainViewModel), new Views.ShellPage());
        }
    }
}
