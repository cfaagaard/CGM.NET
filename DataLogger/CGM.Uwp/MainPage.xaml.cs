using CGM.Communication;
using CGM.Communication.Common;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using CGM.Data.Nightscout.RestApi;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CGM.Data;
using CGM.Data.Nightscout;
using CGM.Web.DataTransfer;
using CGM.Data.Minimed;

namespace CGM.Console.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            CgmApplication cgmApplication = new CgmApplication();
            //add text-file logging
            cgmApplication.AddFileLog(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
#if DEBUG
            //on debug add logging to output
           cgmApplication.AddOutputLog();
#endif

            cgmApplication.AddStateRepsitory();
            cgmApplication.AddDevice<CGM.Uwp.Device.UwpBayerDevice>();

            
            cgmApplication.AddNightscoutBehavoir("---- insert your nightscout url here ", " ----- and the api-key here");

            //not implemented yet
            //cgmApplication.AddWebTransferBehavoir("", "");
            //not implemented yet
            //cgmApplication.AddMiniMedDatabase();

            cgmApplication.Start();




            //IUnityContainer container = new UnityContainer();

            //container.RegisterType<IStateRepository, CGM.Data.SessionStateRepository>();
            //container.RegisterType<IDevice, CGM.Uwp.Device.UwpBayerDevice>(new ContainerControlledLifetimeManager());
            //container.RegisterType<ISessionFactory, MiniMedContext>(new ContainerControlledLifetimeManager());


            //CgmSessionBehaviors behaviors = new CgmSessionBehaviors();
            ////behaviors.SessionBehaviors.Add(container.Resolve<NightScoutUploader>());

            //container.RegisterInstance(typeof(ISessionBehaviors), behaviors);


            //container.RegisterSingleton<ICgmTask, CgmTask>();

            //UnityServiceLocator locator = new UnityServiceLocator(container);
            //ServiceLocator.SetLocatorProvider(() => locator);

            //container.Resolve<ICgmTask>();



        }
    }
}
