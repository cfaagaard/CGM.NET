using CGM.Communication.Common.Serialize;
using CGM.Communication.DTO;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Web.DataTransfer
{
    public class TransferToWeb : ISessionBehavior
    {
        public WebDataTransferConfiguration Configuration { get; set; } = new WebDataTransferConfiguration();

        //public TransferToWeb()
        //{
        //    string configurationFile = $"configurations/WebDataTransferConfiguration.json";
        //    Configuration = JsonConvert.DeserializeObject<WebDataTransferConfiguration>(File.ReadAllText(configurationFile));
        //}
        public async Task ExecuteTask(SerializerSession session, CancellationToken cancelToken)
        {

           //var config= session.GetConfiguration<WebDataTransferConfiguration>();

            var dto = session.GetSessionDTO();
            string key = Configuration.ApiKey.Sha1Digest();
            IFormatter formatter = new BinaryFormatter();

            ActionOfStreamContent content = new ActionOfStreamContent((stream) => formatter.Serialize(stream, dto));
            content.Headers.Add("API-SECRET", Configuration.ApiKey);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.Url);
               var result= client.PostAsync("api/session", content).Result;
            }

        }
    }



}
