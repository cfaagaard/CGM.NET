using CGM.Communication.Common;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;


namespace CGM.Communication
{
    public class CgmApplication : IApplication
    {
        public readonly IUnityContainer unityContainer;
        public CgmSessionBehaviors behaviors;
        public CgmApplication()
        {
            unityContainer = new UnityContainer();

            behaviors = new CgmSessionBehaviors();
            unityContainer.RegisterInstance(typeof(ISessionBehaviors), behaviors);
        }


        public void AddDevice<T>() where T : IDevice
        {
            unityContainer.RegisterType<IDevice, T>(new ContainerControlledLifetimeManager());

            unityContainer.RegisterType<ISessionFactory, MiniMedContext>(new ContainerControlledLifetimeManager());
 
        }

        public void AddContext<T>() where T : ISessionFactory
        {
            
        }

        public void AddBehavior<T>() where T : ISessionBehavior
        {
            behaviors.SessionBehaviors.Add(unityContainer.Resolve<T>());
        }

        public void AddBehavior<T>(T behavior) where T : ISessionBehavior
        {
            behaviors.SessionBehaviors.Add(behavior);
        }

        public void AddFileLog(string path)
        {
            ApplicationLogging.LoggerFactory.AddCgmLog(path);
        }

        public void AddOutputLog()
        {
            ApplicationLogging.LoggerFactory.AddOutputLogger();
        }

        public void AddStaticLog()
        {
            throw new NotImplementedException();
            //ApplicationLogging.LoggerFactory.AddStaticLogger();
        }

        public void Start()
        {

            unityContainer.RegisterSingleton<ICgmTask, CgmTask>();

            UnityServiceLocator locator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);

            unityContainer.Resolve<ICgmTask>();
        }
    }
}
