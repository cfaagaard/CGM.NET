using CGM.Communication.Common.Serialize;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.MiniMed.Model;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CGM.Communication.Extensions;

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

        private bool gotReadingFromEvent = false;
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
            if (string.IsNullOrEmpty(_session.Settings.NightscoutApiUrl) || string.IsNullOrEmpty(_session.Settings.NightscoutSecretkey))
            {
                throw new ArgumentException("Nightscout url or apikey is null.");
            }
            _client = new Data.Nightscout.NightscoutClient(_session.Settings.NightscoutApiUrl, _session.Settings.NightscoutSecretkey);
        }

        public async Task Upload(CancellationToken cancelToken)
        {
            //testing events
            await SyncWithEvents(cancelToken);

            await CreateUploads(cancelToken);

            await UploadElements(cancelToken);
        }

        private async Task CreateUploads(CancellationToken cancelToken)
        {
            if (LastStatusMessage != null)
            {

                if (LastStatusMessage.SgvDateTime.DateTime.HasValue)
                {
                    //getting sgv reading from history now.... hurraaaa
                    //but during testing and harding of the event/history usage, I will do this also
                    if (!gotReadingFromEvent)
                    {
                        await CreateEntrySgv(this.LastStatusMessage.Sgv, this.LastStatusMessage.SgvDateTime.DateTimeString, (long)this.LastStatusMessage.SgvDateTime.DateTimeEpoch, this.LastStatusMessage.CgmTrendName.ToString(), true);
                        if (LastStatusMessage.BolusWizardRecent == 1)
                        {
                            CreateEntryMbg();
                        }
                        //getting it from history....
                        //carbs is not in the statusmessage.
                        CreateCorrectionBolus(this.LastStatusMessage.BolusEstimate, 0, this.LastStatusMessage.SgvDateTime.Rtc.ToString(), this.LastStatusMessage.LastBolusDateTime.ToString(dateformat));
                    }

                    CreateDeviceStatus();
                }

                if (this.LastStatusMessage.Alert != 0 && _session.PumpTime.PumpDateTime.HasValue)
                {
                    CreateAnnouncement($"{this.LastStatusMessage.AlertName.ToString()} - ({this.LastStatusMessage.AlertDateTime})", _session.PumpTime.PumpDateTime.Value, "Alert");
                }
            }
            else
            {
                this.DeviceStatus = null;
            }

        }

        private async Task UploadElements(CancellationToken cancelToken)
        {
            if (Entries.Count > 0)
            {
                await _client.AddEntriesAsync(Entries, cancelToken);
                Logger.LogInformation($"Entries uploaded to Nightscout. ({Entries.Count})");
            }

            if (Treatments.Count > 0)
            {
                //only upload new treatments

                //get double ..... maybe this could be done better
                var getCount = this.Treatments.Count * 2;
                var all = await _client.TreatmentsAsync(null, getCount);
                //remove treatments that are uploaded.

                 var query =
                   from comp in this.Treatments
                   join entry in all on comp.EnteredBy equals entry.EnteredBy
                   select comp;
                query.ToList().ForEach(e => this.Treatments.Remove(e));

                if (this.Treatments.Count>0)
                {
                    await _client.AddTreatmentsAsync(this.Treatments, cancelToken);
                    Logger.LogInformation($"Treatments uploaded to Nightscout. ({Treatments.Count})");
                }
              
            }
            if (this.DeviceStatus != null &&  !string.IsNullOrEmpty(this.DeviceStatus.Device))
            {
                await _client.AddDeviceStatusAsync(new List<Nightscout.DeviceStatus>() { this.DeviceStatus }, cancelToken);
                Logger.LogInformation("DeviceStatus uploaded to Nightscout.");
            }

        }

        private async Task SyncWithEvents(CancellationToken cancelToken)
        {

            var allEvents = Session.PumpDataHistory.JoinAllEvents();
            if (allEvents.Count > 0)
            {
                await MissingReadings(allEvents);
                await MissingWizard(allEvents);
                MissingAlerts(allEvents);
                SensorChange(allEvents);
           
                //Get cannula fill -> microsoft flow

                //Alarm -> microsoft flow
            }

        }

        private void SensorChange(IEnumerable<PumpEvent> allEvents)
        {
            var events = allEvents.Where(e => e.EventType == MiniMed.Infrastructur.EventTypeEnum.GLUCOSE_SENSOR_CHANGE);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    CgmSensorChanged(item.Timestamp.Value);
                }
            }
        }

        private void MissingAlerts(IEnumerable<PumpEvent> allEvents)
        {
            var events = allEvents.Where(e => e.EventType == MiniMed.Infrastructur.EventTypeEnum.ALARM_NOTIFICATION);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    var msg = (ALARM_NOTIFICATION_Event)item.Message;

                    CreateAnnouncement($"{msg.AlarmTypeName.ToString()} - ({msg.Timestamp.Value.ToString()})", msg.Timestamp.Value,"Alert");

                }
            }
        }

        private async Task MissingWizard(IEnumerable<PumpEvent> allEvents)
        {
            //BOLUS_WIZARD_ESTIMATE_Event
            var wizards = allEvents.Where(e => e.EventType == MiniMed.Infrastructur.EventTypeEnum.BOLUS_WIZARD_ESTIMATE);
            int count = wizards.Count();
            if (count > 0)
            {
                foreach (var item in wizards)
                {
                    var msg = (BOLUS_WIZARD_ESTIMATE_Event)item.Message;

                    CreateCorrectionBolus(msg.FinalEstimate.INSULIN, msg.CARB_INPUT.CARB, item.Rtc.ToString(), item.Timestamp.Value.ToString(dateformat));

                }
            }
        }

        private async Task MissingReadings(IEnumerable<PumpEvent> allEvents)
        {
            var sensorReadings = allEvents.Where(e => e.EventType == MiniMed.Infrastructur.EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED);
            int count = sensorReadings.Count();
            if (count > 0)
            {
                gotReadingFromEvent = true;
                List<CompareEvents> compares = new List<CompareEvents>();

                foreach (var pumpevent in sensorReadings)
                {
                    var firstdate = pumpevent.Timestamp.Value;
                    var reading = (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)pumpevent.Message;
                    for (int i = 0; i < reading.Details.Count; i++)
                    {
                        //if (reading.Details[i].Amount <= 400)
                        //{
                        var readDateTime = firstdate.AddMinutes(i * -5);
                        compares.Add(new CompareEvents(reading.Details[i], readDateTime));
                        //}
                    }
                }
                //todo another query by dates would be better. a between query.... but nightscout restapi do not have this...or it does not work.
                var from = sensorReadings.First().Timestamp;
                var to = sensorReadings.Last().Timestamp;

                var numberOfDays = to.Value.Subtract(from.Value).Days;
                List<Entry> entries = new List<Entry>();

                //for (int i = 0; i <= numberOfDays; i++)
                //{
                //    DateTime getdate = from.Value.AddDays(i);

                //    var allTodayEntries = await _client.EntriesAsync($"find[type]=sgv&find[dateString][$gte]={getdate.ToString("yyyy-MM-dd")}", 1000);
                //    entries.AddRange(allTodayEntries);
                //}


                var allTodayEntries = await _client.EntriesAsync($"find[type]=sgv", 1000);
                entries.AddRange(allTodayEntries);

                var query =
                   from comp in compares
                   join entry in entries on comp.DateString equals entry.DateString
                   select comp;

                var missingReadings = compares.Except(query.ToList()).ToList();

                foreach (var reading in missingReadings.OrderBy(e => e.ReadingTime))
                {
                    //TODO need to find direction in the history, maybe rate of change....
                    await CreateEntrySgv(reading.Detail.Amount, reading.DateString, reading.Epoch, reading.Detail.Trend.ToString(), false);
                }

            }
        }

        class CompareEvents
        {
            public string DateString { get; set; }
            public DateTime ReadingTime { get; set; }
            public long Epoch { get; set; }
            public SENSOR_GLUCOSE_READINGS_EXTENDED_Detail Detail { get; set; }
            public CompareEvents(SENSOR_GLUCOSE_READINGS_EXTENDED_Detail detail, DateTime readingTime)
            {
                this.Detail = detail;
                this.ReadingTime = readingTime;
                DateTimeOffset utcTime2 = this.ReadingTime;
                Epoch = utcTime2.ToUnixTimeMilliseconds();
                DateString = readingTime.ToString("ddd, MMM dd HH:mm:ss CEST yyyy", CultureInfo.InvariantCulture);
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

        protected async Task CreateEntrySgv(int sgvValue, string dateString, long epoch, string direction, bool checkIfExists)
        {

            string serialNum = _session.Device.SerialNumberFull;
            Entry entry = new Entry();
            entry.Date = epoch;
            entry.Direction = direction;
            entry.Sgv = sgvValue;
            entry.Type = "sgv";
            entry.DateString = dateString;
            entry.Device = string.Format("medtronic-640g://{0}", serialNum);

            if (entry.Sgv <= 400)
            {
                if (checkIfExists)
                {
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
                    this.Entries.Add(entry);
                }
            }
            else
            {
                CreateSgvEntryAlert(entry);
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

        private void CreateCorrectionBolus(double insulin, double carbs, string reference, string dateTime)
        {
            Treatment treatment = new Treatment();
            treatment.Insulin = insulin;// message.BolusEstimate;
            treatment.Carbs = carbs;
            treatment.EventType = "Correction Bolus";
            treatment.EnteredBy = $"Ref:{reference}";// $"Ref:{message.LastBolusReference}";
            treatment.Created_at = dateTime;// message.LastBolusDateTime.ToString(dateformat);
            Treatments.Add(treatment);

            //var last = await _client.TreatmentsAsync($"find[eventType]={treatment.EventType.Replace(" ", "+")}&find[enteredBy]={treatment.EnteredBy}", 1);
            //if (last.Count == 0)
            //{
                
            //}
        }

        private void CreateNote(string note, string datestring)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Note";
            treatment.Created_at = datestring;
            treatment.Notes = note;
            Treatments.Add(treatment);

        }

        private void CreateSgvEntryAlert(Entry entry)
        {
            if (entry.Sgv.HasValue)
            {
                var alert = (Alerts)entry.Sgv.Value;

                var date = entry.Date.Value.GetDateTime();
                var note = $"{alert.ToString()} - ({entry.DateString})";

                CreateAnnouncement(note, date, "SgvAlert");

            }

        }

        private void CreateAnnouncement(string note, DateTime createdAt, string reference)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Announcement";
            treatment.Created_at = createdAt.ToString(dateformat);
            treatment.Notes = note;
            treatment.EnteredBy = $"ref:${reference} - {treatment.Created_at}";
            Treatments.Add(treatment);

        }

        private void CgmSensorChanged(DateTime createdAt)
        {

            Treatment treatment = new Treatment();
            treatment.EventType = "Sensor Change";
            treatment.Created_at = createdAt.ToString(dateformat);
            treatment.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment);


            Treatment treatment2 = new Treatment();
            treatment2.EventType = "Sensor Start";
            treatment2.Created_at = createdAt.ToString(dateformat);
            treatment2.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment2);


        }

        private void InsulinChanged(DateTime createdAt)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Insulin Change";
            treatment.Created_at = createdAt.ToString(dateformat);
            treatment.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment);
        }

        private void CannulaChanged(DateTime createdAt)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Site Change";
            treatment.Created_at = createdAt.ToString(dateformat);
            treatment.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment);
        }
    }
}
