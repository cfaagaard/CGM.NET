using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Web.DataTransfer
{
    public class WebDataTransferConfiguration:IConfiguration
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }


    }
}
