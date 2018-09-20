using System;
using System.Collections.Generic;
using System.Text;
using Unity;
namespace CGM.Web.DataTransfer
{
    public static class ApplicationExtension
    {
        public static void AddWebTransferBehavoir(this CGM.Communication.CgmApplication cgmApplication, string url, string apiKey)
        {
            TransferToWeb uploader = cgmApplication.unityContainer.Resolve<TransferToWeb>();
            uploader.Configuration.Url = url;
            uploader.Configuration.ApiKey = apiKey;
            cgmApplication.AddBehavior(uploader);

        }
    }
}
