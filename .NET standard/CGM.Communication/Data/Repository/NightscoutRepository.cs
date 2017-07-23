using CGM.Communication.Data.Nightscout;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task UploadToNightScout(string url, string apiKey, Nightscout.Entry entrySgv, Nightscout.DeviceStatus deviceStatus, Nightscout.Entry entryMgb)
        {

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("Nightscout url or apikey is null.");
            }

            try
            {
                CGM.Communication.Data.Nightscout.NightscoutClient client = new Data.Nightscout.NightscoutClient(url, apiKey);

                var last = await client.EntriesAsync(null, 1);
                if (last.Count == 0 || !entrySgv.DateString.Equals(last[0].DateString))
                {
                    if (entryMgb != null)
                    {
                        //only mgb in order to display it on the graph.
                        await client.AddEntriesAsync(new List<Nightscout.Entry>() { entryMgb });
                        Logger.LogInformation("Mgb-entry uploaded to Nightscout.");
                    }

                    //post the sgv
                    await client.AddEntriesAsync(new List<Nightscout.Entry>() { entrySgv });
                    Logger.LogInformation("Sgv-entry uploaded to Nightscout.");

                }
                else
                {
                    Logger.LogInformation("Entry not uploaded to Nightscout. Already exists.");
                }


                await client.AddDeviceStatusAsync(new List<Nightscout.DeviceStatus>() { deviceStatus });
                Logger.LogInformation("DeviceStatus uploaded to Nightscout.");

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
