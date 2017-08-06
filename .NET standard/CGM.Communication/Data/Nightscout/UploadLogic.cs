using CGM.Communication.Common.Serialize;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Data.Nightscout
{
    public class UploadLogic
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<UploadLogic>();

        private SerializerSession _session;
        private CGM.Communication.Data.Nightscout.NightscoutClient _client;
        private string dateformat = "yyyy-MM-ddTHH\\:mm\\:sszzz";
        //private string treatmentDateformat = "yyyy-MM-ddTHH\\:mm\\:ss.000Z";

        public List<Treatment> Treatments { get; set; } = new List<Treatment>();
        public List<Entry> Entries { get; set; } = new List<Entry>();
        public DeviceStatus DeviceStatus { get; set; } = new DeviceStatus();

        public SerializerSession Session { get { return _session; } }

        private PumpStatusMessage _lastStatusMessage;
        public PumpStatusMessage LastStatusMessage
        {
            get
            {
                if (_session.Status.Count > 0 && _lastStatusMessage == null)
                {
                    _lastStatusMessage = _session.Status.Last();
                }
                return _lastStatusMessage;
            }
        }

        public UploadLogic(SerializerSession session)
        {
            _session = session;

        }

        public async Task CreateUploads(CancellationToken cancelToken)
        {
            if (LastStatusMessage != null)
            {
                if (string.IsNullOrEmpty(_session.Settings.NightscoutApiUrl) || string.IsNullOrEmpty(_session.Settings.NightscoutSecretkey))
                {
                    throw new ArgumentException("Nightscout url or apikey is null.");
                }
                _client = new Data.Nightscout.NightscoutClient(_session.Settings.NightscoutApiUrl, _session.Settings.NightscoutSecretkey);

                await CreateEntrySgv();

                if (LastStatusMessage.BolusWizardRecent == 1)
                {
                    CreateEntryMbg();
                }

                CreateDeviceStatus();

                await CreateCorrectionBolusTreatment();
            }
            else
            {
                this.DeviceStatus = null;
            }

        }

        public async Task Upload(CancellationToken cancelToken)
        {
            if (Entries.Count > 0)
            {
                await _client.AddEntriesAsync(Entries, cancelToken);
                Logger.LogInformation($"Entries uploaded to Nightscout.");
            }

            if (Treatments.Count > 0)
            {
                await _client.AddTreatmentsAsync(this.Treatments, cancelToken);
                Logger.LogInformation($"Treatments uploaded to Nightscout.");
            }
            if (this.DeviceStatus != null)
            {
                await _client.AddDeviceStatusAsync(new List<Nightscout.DeviceStatus>() { this.DeviceStatus }, cancelToken);
                Logger.LogInformation("DeviceStatus uploaded to Nightscout.");
            }

            if (this.LastStatusMessage.Alert!=0)
            {
                CreateAnnouncement($"{this.LastStatusMessage.AlertName.ToString()} - ({this.LastStatusMessage.AlertDateTime})");
            }

        }

        protected void CreateEntryMbg()
        {
            string serialNum = _session.Device.SerialNumberFull;
            PumpStatusMessage message = this.LastStatusMessage;

            Entry entry = new Entry();
            entry.Date = message.SgvDateTime.DateTimeEpoch;
            entry.Mbg = message.BolusWizardBGL;
            entry.Type = "mbg";
            entry.DateString = message.SgvDateTime.DateTime.Value.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");
            entry.Device = string.Format("medtronic-640g://{0}", serialNum);
            this.Entries.Add(entry);
        }

        protected async Task CreateEntrySgv()
        {

            string serialNum = _session.Device.SerialNumberFull;
            PumpStatusMessage message = this.LastStatusMessage;
            int sgvValue = message.Sgv;
            DateTimeDataType date = message.SgvDateTime;

            if (date.DateTime.HasValue)
            {


                if (sgvValue == 769)
                {
                    //do not upload sgv, but create note.
                    //observed during warmup after changing the pump.
                    CreateNote("Warmup....");
                    return;
                }

                if (sgvValue == 770)
                {
                    //observed during alert "need calibration"
                    CreateNote("Need calibration.....");
                    return;
                }
                if (sgvValue > 400)
                {
                    sgvValue = 400;
                }
                Entry entry = new Entry();
                entry.Date = date.DateTimeEpoch;
                entry.Direction = message.CgmTrendName.ToString();
                entry.Sgv = sgvValue;
                entry.Type = "sgv";
                entry.DateString = date.DateTime.Value.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");
                entry.Device = string.Format("medtronic-640g://{0}", serialNum);

                var last = await _client.EntriesAsync(null, 1);
                if (last.Count == 0 || !entry.DateString.Equals(last[0].DateString))
                {
                    this.Entries.Add(entry);
                }
                else
                {
                    Logger.LogInformation("Entry not uploaded to Nightscout. Already exists.");
                }
            }
            else
            {
                Logger.LogInformation("No sgv-date.");
                //sending a note to nightscout, if no alert. If there is a alert another note will be send.
                if (this.LastStatusMessage.Alert==0)
                {
                    CreateNote("No sgv-date.");
                }
                
            }
        }

        private void CreateDeviceStatus()
        {

            PumpStatusMessage message = this.LastStatusMessage;
            string serialNum = _session.Device.SerialNumberFull;

            int uploadBattery = 100;
            DateTime create = _session.PumpTime.PumpDateTime.Value;

            this.DeviceStatus = new DeviceStatus();

            this.DeviceStatus.UploaderBattery = uploadBattery;
            this.DeviceStatus.Device = string.Format("medtronic-640g://{0}", serialNum);
            this.DeviceStatus.CreatedAt = create.ToString(dateformat);
            this.DeviceStatus.PumpInfo.Reservoir = Math.Round(message.ReservoirAmount, 3);
            this.DeviceStatus.PumpInfo.Iob.Bolusiob = Math.Round(message.ActiveInsulin.INSULIN, 3);
            this.DeviceStatus.PumpInfo.Iob.Timestamp = create.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");

            this.DeviceStatus.PumpInfo.Clock = create.ToString(dateformat);
            this.DeviceStatus.PumpInfo.Battery.Percent = message.BatteryPercentage;

            if (message.Status.Suspended)
            {
                this.DeviceStatus.PumpInfo.Status.Add("supended", true);
            }
            if (message.Status.Bolusing)
            {
                this.DeviceStatus.PumpInfo.Status.Add("bolusing", true);
            }

            if (!message.Status.Suspended && !message.Status.Bolusing)
            {
                this.DeviceStatus.PumpInfo.Status.Add("status", "normal");
            }

        }

        private async Task CreateCorrectionBolusTreatment()
        {


            PumpStatusMessage message = this.LastStatusMessage;
            Treatment treatment = new Treatment();
            treatment.Insulin = message.BolusEstimate;
            //need to be in the same measurement as nightscout...... set to mmol for now.
            treatment.Glucose = message.SgvMmol.ToString();
            treatment.EventType = "Correction Bolus";
            treatment.EnteredBy = $"Ref:{message.LastBolusReference}";
            treatment.Created_at = message.LastBolusDateTime.ToString(dateformat);

            var last = await _client.TreatmentsAsync($"find[eventType]={treatment.EventType.Replace(" ", "+")}&find[enteredBy]={treatment.EnteredBy}", 1);
            if (last.Count == 0)
            {
                Treatments.Add(treatment);
            }
        }

        private void CreateNote(string note)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Note";
            treatment.Created_at = _session.PumpTime.PumpDateTime.Value.ToString(dateformat);
            treatment.Notes = note;
            Treatments.Add(treatment);

        }

        private void CreateAnnouncement(string note)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Announcement";
            treatment.Created_at = _session.PumpTime.PumpDateTime.Value.ToString(dateformat);
            treatment.Notes = note;
            Treatments.Add(treatment);

        }
        
    }
}
