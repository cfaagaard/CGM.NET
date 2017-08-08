using CGM.Communication.Common.Serialize;
using CGM.Communication.Data.Nightscout;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Data.Repository
{
    public class NightscoutRepository
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<NightscoutRepository>();
        private CgmUnitOfWork _uow;
        public NightscoutRepository(CgmUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Profile> GetProfil(string url, string apiKey)
        {
            CGM.Communication.Data.Nightscout.NightscoutClient client = new Data.Nightscout.NightscoutClient(url, apiKey);
            return await client.ProfileAsync();
        }

        public async Task UpdateBasalProfil(string url, string apiKey, Profile profile)
        {
            throw new NotImplementedException();
        }

        public async Task UploadToNightScout(SerializerSession session, CancellationToken cancelToken)
        {
            UploadLogic upLogic = new UploadLogic(session);
            await upLogic.Upload(cancelToken);

        }



    }
}
