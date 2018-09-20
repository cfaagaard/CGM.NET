using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace CGM.Data.Nightscout
{
    public static class ApplicationExtension
    {
        public static void AddNightscoutBehavoir(this CGM.Communication.CgmApplication cgmApplication, string url, string apiKey)
        {
            RestApi.NightScoutUploader uploader = cgmApplication.unityContainer.Resolve<RestApi.NightScoutUploader>();
            uploader.NightscoutConfiguration = new NightscoutConfiguration() { NightscoutUrl = url, NightscoutSecretkey = apiKey };
            cgmApplication.AddBehavior(uploader);

        }
    }
}
