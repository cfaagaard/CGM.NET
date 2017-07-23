
using CGM.Communication.Data;
using CGM.Communication.Data.Nightscout;
using CGM.Communication.Data.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Model;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Interfaces;

namespace CGM.Communication.Data.Repository
{
    public class PumpRepository
    {

        private CgmUnitOfWork _uow;
        public PumpRepository(CgmUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SerializerSession> GetPumpSessionAsync(IDevice device, CancellationToken cancelToken)
        {
            if (device == null)
            {
                throw new ArgumentException("No device found");
            }
            MiniMed.MiniMedContext context = new MiniMed.MiniMedContext(device);

            return await context.GetPumpSessionAsync(cancelToken);

        }

        public async Task<SerializerSession> GetPumpDataAndUploadAsync(IDevice device, int uploaderBattery, CancellationToken cancelToken)
        {
            try
            {
                SerializerSession session = await GetPumpSessionAsync(device, cancelToken);
                if (session != null)
                {
                    session.Battery = uploaderBattery;
                    if (!cancelToken.IsCancellationRequested)
                    {
                        await SaveSession(session);

                        if (session.RadioChannel != 0x00)
                        {
                            await UploadToNightscout(session).TimeoutAfter(5000);
                        }
                    }
                }
                return session;
            }
            catch (Exception)
            {

                throw;
            }

        }



        public async Task UploadToNightscout(SerializerSession session)
        {
            if (session.Status.Count > 0)
            {
                var message = session.Status.Last();


                if (message.Sgv != 0)
                {
                    Entry entrySgv = MapToEntrySgv(message, session.Device.SerialNumberFull);
                    Entry entryMgb = MapToEntryMbg(message, session.Device.SerialNumberFull);
                    DeviceStatus deviceStatus = MapToDeviceStatus(message, session.Device.SerialNumberFull, 100, session.PumpTime.PumpDateTime.Value);


                    if (message.BolusWizardRecent == 1)
                    {
                        await _uow.Nightscout.UploadToNightScout(session.Settings.NightscoutApiUrl, session.Settings.NightscoutSecretkey, entrySgv, deviceStatus, entryMgb);
                    }
                    else
                    {
                        await _uow.Nightscout.UploadToNightScout(session.Settings.NightscoutApiUrl, session.Settings.NightscoutSecretkey, entrySgv, deviceStatus, null);
                    }


                }
            }

        }

        public async Task SaveSession(SerializerSession session)
        {
            _uow.Device.AddUpdateSessionToDevice(session);
        }

        protected Communication.Data.Nightscout.Entry MapToEntryMbg(PumpStatusMessage message, string serialNum)
        {
            Entry entry = new Entry();
            entry.Date = message.SgvDateTimeEpoch;
            entry.Mbg = message.BolusWizardBGL;
            entry.Type = "mbg";
            entry.DateString = message.SgvDateTime.Value.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");
            entry.Device = string.Format("medtronic-640g://{0}", serialNum);

            return entry;
        }

        protected Communication.Data.Nightscout.Entry MapToEntrySgv(PumpStatusMessage message, string serialNum)
        {

            int sgvValue = message.Sgv;
            if (sgvValue > 400)
            {
                sgvValue = 400;
            }
            Communication.Data.Nightscout.Entry entry = new Communication.Data.Nightscout.Entry();
            entry.Date = message.SgvDateTimeEpoch;
            entry.Direction = message.CgmTrendName.ToString();
            entry.Sgv = sgvValue;
            entry.Type = "sgv";
            entry.DateString = message.SgvDateTime.Value.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");
            entry.Device = string.Format("medtronic-640g://{0}", serialNum);

            return entry;
        }

        private DeviceStatus MapToDeviceStatus(PumpStatusMessage message, string serialNum, int uploadBattery, DateTime create)
        {
            //2017-05-14T22:38:35+0200
            string dateformat = "yyyy-MM-ddTHH\\:mm\\:sszzz";

            DeviceStatus status = new DeviceStatus();

            status.UploaderBattery = uploadBattery;
            status.Device = string.Format("medtronic-640g://{0}", serialNum);
            status.CreatedAt = create.ToString(dateformat);
            status.PumpInfo.Reservoir = Math.Round(message.ReservoirAmount, 3);
            status.PumpInfo.Iob.Bolusiob = Math.Round(message.ActiveInsulin, 3);
            status.PumpInfo.Iob.Timestamp = create.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");

            status.PumpInfo.Clock = create.ToString(dateformat);
            status.PumpInfo.Battery.Percent = message.BatteryPercentage;

            if (message.Status.Suspended)
            {
                status.PumpInfo.Status.Add("supended", true);
            }
            if (message.Status.Bolusing)
            {
                status.PumpInfo.Status.Add("bolusing", true);
            }

            if (!message.Status.Suspended && !message.Status.Bolusing)
            {
                status.PumpInfo.Status.Add("status", "normal");
            }

            return status;
        }
    }
}
