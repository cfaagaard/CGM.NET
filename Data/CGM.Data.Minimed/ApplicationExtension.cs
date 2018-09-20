using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace CGM.Data.Minimed
{
    public static class ApplicationExtension2
    {
        public static void AddMiniMedDatabase(this CGM.Communication.CgmApplication cgmApplication)
        {
            MiniMedRepository miniMedRepository = cgmApplication.unityContainer.Resolve<MiniMedRepository>();
            miniMedRepository.OnStartUp();

            cgmApplication.behaviors.SessionBehaviors.Add(miniMedRepository);
        }
    }
}
