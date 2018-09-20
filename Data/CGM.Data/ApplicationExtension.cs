using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace CGM.Data
{
    public static class ApplicationExtension
    {
        public static void AddStateRepsitory(this CGM.Communication.CgmApplication cgmApplication)
        {
            cgmApplication.unityContainer.RegisterType<IStateRepository, CGM.Data.SessionStateRepository>();
        }
    }
}
