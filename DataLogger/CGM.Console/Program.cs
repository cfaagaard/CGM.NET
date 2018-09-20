using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using CGM.Data.Nightscout.RestApi;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using CGM.Communication.Extensions;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CGM.Communication.DTO;
using CGM.Web.DataTransfer;
using CGM.Communication;
using CGM.Data;
using CGM.Data.Nightscout;

namespace CGM.Console
{
    class Program
    {
        static void Main(string[] args)
        {


            CgmApplication cgmApplication = new CgmApplication();
            //add text-file logging
            cgmApplication.AddFileLog("Logs");
#if DEBUG
            //on debug add logging to output
            cgmApplication.AddOutputLog();
#endif

            cgmApplication.AddStateRepsitory();
            cgmApplication.AddDevice<CGM.Hid.Device.HidBayerDevice>();

            cgmApplication.AddNightscoutBehavoir("---- insert your nightscout url here ", " ----- and the api-key here");

            cgmApplication.Start();

            System.Console.ReadLine();




        }
    }
}
