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
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Common;
using CGM.Communication.Interfaces;

namespace CGM.Data.Nightscout.RestApi
{

    public class NightScoutUploader : ISessionBehavior
    {

        private string dateformat = "yyyy-MM-ddTHH\\:mm\\:sszzz";
        protected ILogger Logger = ApplicationLogging.CreateLogger<NightScoutUploader>();

        protected SerializerSession _session;
        protected NightscoutClient _client;
        public NightscoutConfiguration NightscoutConfiguration { get; set; } = new NightscoutConfiguration();
        protected NotifiyConfiguration _notifiyConfiguration = new NotifiyConfiguration();


        public List<Treatment> Treatments { get; set; } = new List<Treatment>();
        public List<Entry> Entries { get; set; } = new List<Entry>();
        public DeviceStatus DeviceStatus { get; set; } = new DeviceStatus();
        public SerializerSession Session { get { return _session; } }

        private PumpStatusMessage _lastStatusMessage;
        private readonly IStateRepository repository;
        private List<BasePumpEvent> eventsToHandle;

        public PumpStatusMessage LastStatusMessage
        {
            get
            {
                if (_session == null)
                {
                    return null;
                }
                if (_session.Status.Count > 0 && _lastStatusMessage == null)
                {
                    _lastStatusMessage = _session.Status.Last();
                }
                return _lastStatusMessage;
            }
        }

        public NightScoutUploader(IStateRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task ExecuteTask(SerializerSession session, CancellationToken cancelToken)
        {
            //NightscoutConfiguration = session.NightscoutConfiguration();
            //_notifiyConfiguration = session.NotifiyConfiguration();
            _session = session;

            if (session.GotConnectionToPump)
            {
                eventsToHandle = GetHistoryWithNoStatus();

                CreateDeviceStatus();
                AddDeviceStatus(cancelToken);

                if (eventsToHandle.Count > 0)
                {
                    Readings();
                    Wizard();
                    Alerts();
                    TempBasalProgrammed();
                    SensorChange();
                    CannulaChanged();
                    BgReadings();
                    AirplaneMode();
                    BatteryChanged();
                    ExerciseMarker();
                    //DailyTotals();

                    this.Treatments.Where(e => e.EventType == "Announcement").ToList().ForEach(e => e.IsAnnouncement = true);


                    AddEntries(cancelToken);
                    AddTreatments(cancelToken);

                    var keys = eventsToHandle.GroupBy(e => e.Key).Select(e=>e.Key).ToList();
                    repository.AddKeys(keys);

                    await Notify(cancelToken);

                }
                else
                {
                    Logger.LogInformation($"No new pump-events.");
                }
            }
            else
            {
                Logger.LogInformation("No data uploaded to Nightscout");
            }



        }

        protected List<BasePumpEvent> GetHistoryWithNoStatus()
        {
            return repository.GetHistoryWithNoStatus(Session);
        }


        protected List<BasePumpEvent> GetHistory(List<int> eventFilter)
        {
            return repository.GetHistory(eventFilter, Session);
        }

        protected async void AddEntries(CancellationToken cancelToken)
        {
            if (Entries.Count > 0)
            {
                try
                {

                    await _client.AddEntriesAsync(Entries, cancelToken);

                    Logger.LogInformation($"Entries uploaded to Nightscout. ({Entries.Count})");
                    //var keys = Entries.Select(e => e.Key).ToList();

                    //var keys = Entries.GroupBy(e => e.Key).Select(e=>e.Key).ToList();
                    //repository.AddKeys(keys);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        protected async void AddTreatments(CancellationToken cancelToken)
        {
            if (this.Treatments.Count > 0)
            {
                try
                {
                    await _client.AddTreatmentsAsync(this.Treatments, cancelToken);
                    Logger.LogInformation($"Treatments uploaded to Nightscout. ({Treatments.Count})");

                    //var keys = Treatments.Select(e => e.Key).ToList();
                    //var keys = Treatments.GroupBy(e => e.Key).Select(e => e.Key).ToList();
                    //repository.AddKeys(keys);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        protected async void AddDeviceStatus(CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(NightscoutConfiguration.NightscoutApiUrl) || string.IsNullOrEmpty(NightscoutConfiguration.NightscoutSecretkey))
            {
                throw new ArgumentException("Nightscout url or apikey is null.");
            }
            _client = new NightscoutClient(NightscoutConfiguration.NightscoutApiUrl, NightscoutConfiguration.NightscoutSecretkey);

            if (this.DeviceStatus != null && !string.IsNullOrEmpty(this.DeviceStatus.Device))
            {
                await _client.AddDeviceStatusAsync(new List<DeviceStatus>() { this.DeviceStatus }, cancelToken);
                Logger.LogInformation("DeviceStatus uploaded to Nightscout.");
            }
        }

        private void TempBasalProgrammed()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.TEMP_BASAL_PROGRAMMED);
            foreach (var item in events)
            {

                var treatment = CreateTempBasalProgrammed(item.EventDate.DateTime.Value, item.Key);
                var msg = (TEMP_BASAL_PROGRAMMED_Event)item.Message;

                treatment.Duration = msg.Duration.ToString();
                treatment.Percent = msg.Percentage.ToString();
            }
        }

        private void AirplaneMode()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.AIRPLANE_MODE);
            foreach (var item in events)
            {
                string status = "On";
                if (item.Message.AllBytes[0] == 0x00)
                {
                    status = "Off";
                }
                CreateNote(string.Format("{0} ({1})", EventTypeEnum.AIRPLANE_MODE.ToString(), status), item.EventDate.DateTime.Value, item.Key);

            }
        }

        private void BatteryChanged()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.BATTERY_INSERTED);
            foreach (var item in events)
            {
                CreateBattery(EventTypeEnum.BATTERY_INSERTED.ToString(), item.EventDate.DateTime.Value, item.Key);

            }
        }

        private void ExerciseMarker()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.EXERCISE_MARKER);
            foreach (var item in events)
            {
                var msg = (EXERCISE_MARKER_Event)item.Message;
                CreateExercise(EventTypeEnum.EXERCISE_MARKER.ToString(), item.EventDate.DateTime.Value, item.Key, msg.Duration);

            }
        }

        private void DailyTotals()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.DAILY_TOTALS);
            foreach (var item in events)
            {
                var msg = (DAILY_TOTALS_Event)item.Message;
                CreateDailyTotal(msg, item.EventDate.DateTime.Value, item.Key);

            }
        }

        private async Task Notify(CancellationToken cancelToken)
        {
            if (!string.IsNullOrEmpty(_notifiyConfiguration.NotificationUrl))
            {
                var notif = this.Treatments.Where(e => !string.IsNullOrEmpty(e.Notification.Type)).Select(e => e.Notification);
                if (notif.Count() > 0)
                {
                    NotificationClient client = new NotificationClient(_notifiyConfiguration.NotificationUrl);
                    foreach (var item in notif)
                    {
                        await client.AddNotificationAsync(item, cancelToken);
                    }
                    Logger.LogInformation($"Notifications sent. ({notif.Count()})");
                }

            }
        }

        private void SensorChange()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.GLUCOSE_SENSOR_CHANGE);
            foreach (var item in events)
            {
                var treatment = CreateCgmSensorChanged(item.EventDate.DateTime.Value, item.Key);
                treatment.Notification.Date = item.EventDate.DateTime.Value.ToString();
                treatment.Notification.Type = EventTypeEnum.GLUCOSE_SENSOR_CHANGE.ToString();
                treatment.Notification.Text = EventTypeEnum.GLUCOSE_SENSOR_CHANGE.ToString();
            }
        }

        private void CannulaChanged()
        {
            //is this correct?
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.CANNULA_FILL_DELIVERED && ((CANNULA_FILL_DELIVERED_Event)e.Message).PRIME_TYPE_NAME == PrimeTypeEnum.canulla_fill);

            foreach (var item in events)
            {
                var msg = (CANNULA_FILL_DELIVERED_Event)item.Message;

                //insulin changed
                CreateInsulinChanged(item.EventDate.DateTime.Value, item.Key);

                //if any amount of insulin is filled, then there was a cannula change
                if (msg.AMOUNT.Insulin > 0)
                {
                    var cannulaTreatment = CreateCannulaChanged(item.EventDate.DateTime.Value, item.Key);
                    cannulaTreatment.Notification.Date = item.EventDate.DateTime.Value.ToString();
                    cannulaTreatment.Notification.Type = EventTypeEnum.CANNULA_FILL_DELIVERED.ToString();
                    cannulaTreatment.Notification.Text = EventTypeEnum.CANNULA_FILL_DELIVERED.ToString();
                }
            }
        }

        private void Alerts()
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.ALARM_NOTIFICATION);

            foreach (var item in events)
            {
                var msg = (ALARM_NOTIFICATION_Event)item.Message;

                var announcement = CreateAnnouncement($"{msg.AlarmTypeName.ToString()}", msg.EventDate.DateTime.Value, "Alert", item.Key);
                //announcement.Notification.Date = msg.Timestamp.Value.ToString();
                //announcement.Notification.Type = EventTypeEnum.ALARM_NOTIFICATION.ToString();
                //announcement.Notification.Text = msg.AlarmTypeName.ToString();

                if (msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Suspend_Before_Low_Alarm_quiet_810
                    || msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Suspend_Before_Low_Alarm_811
                     || msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Suspend_On_Low_Alarm_809
                    )
                {
                    var treatment = CreateTempBasalProgrammed(item.EventDate.DateTime.Value, item.Key);

                    treatment.Duration = "60";
                    treatment.absolute = "0";
                }

                if (msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Basal_Delivery_Resumed_Alert_quiet_806
                    || msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Basal_Delivery_Resumed_Alert_glucose_still_low_maximum_suspend_reached_814
                    || msg.AlarmTypeName == Communication.MiniMed.Model.Alerts.Basal_Delivery_Resumed_Alert_maximum_suspend_reached_808)
                {
                    var treatment = CreateTempBasalProgrammed(item.EventDate.DateTime.Value, item.Key);

                    treatment.Duration = "0";
                }
            }
        }

        private void Wizard()
        {
            List<BasePumpEvent> wizards = GetHistory(new List<int>() { (int)EventTypeEnum.BOLUS_WIZARD_ESTIMATE }).OrderBy(e=>e.EventDate.DateTime).ToList();
            var bolusDelivered = eventsToHandle.Where(e => e.EventType == EventTypeEnum.NORMAL_BOLUS_DELIVERED).ToList();


            foreach (var item in bolusDelivered)
            {
                var bolusevent = (item.Message as NORMAL_BOLUS_DELIVERED_Event);

                if (bolusevent.bolusSourceName==BolusSourceEnum.BOLUS_WIZARD)
                {

                    var wizard = wizards.FirstOrDefault(e =>
                    (e.Message as BOLUS_WIZARD_ESTIMATE_Event).FinalEstimate.InsulinRaw == bolusevent.ProgrammedAmount.InsulinRaw
                    && e.EventDate.DateTime < bolusevent.EventDate.DateTime);
                    if (wizard != null)
                    {
                        var msg = (BOLUS_WIZARD_ESTIMATE_Event)wizard.Message;
                        if (bolusevent.DeliveredAmount.Insulin != 0 || msg.CarbInput != 0)
                        {
                            CreateCorrectionBolus(bolusevent.DeliveredAmount.Insulin, msg.CarbInput, bolusevent.EventDate.Rtc.ToString(), bolusevent.EventDate.DateTime.Value.ToString(Constants.Dateformat), item.Key);
                        }
                    }
                    else
                    {

                    }
                }

            }
        }

        private void Readings()
        {
            var sensorReadings = eventsToHandle.Where(e => e.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED).ToList();
            List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> details = new List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>();

            foreach (var msg in sensorReadings)
            {
                var reading = (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)msg.Message;
                for (int i = 0; i < reading.Details.Count; i++)
                {
                    var read = reading.Details[i];
                    CreateEntrySgv(read.Amount, read.EventDate.DateTime.Value.ToString(dateformat), read.EventDate.DateTimeEpoch, Trend(read.RateOfChange, read.PredictedSg).ToString(), false, msg.Key, read.PredictedSg, read.Isig);
                }
            }
        }

        public SgvTrend Trend(double RateOfChangeRaw, int PredictedSg)
        {
            SgvTrend _trend;
            //should implement this: http://journals.sagepub.com/doi/full/10.1177/1932296817723260
            _trend = SgvTrend.NotComputable;
            if (RateOfChangeRaw == 0 && PredictedSg == 0)
            {
                _trend = SgvTrend.NotComputable;
            }
            else
            {
                //maybe there is a max
                if (RateOfChangeRaw > 300)
                {
                    _trend = SgvTrend.DoubleUp;
                }
                if (RateOfChangeRaw <= 300 && RateOfChangeRaw >= 100)
                {
                    _trend = SgvTrend.SingleUp;
                }

                if (RateOfChangeRaw <= 101 && RateOfChangeRaw >= 51)
                {
                    _trend = SgvTrend.FortyFiveUp;
                }


                if (RateOfChangeRaw <= 50 && RateOfChangeRaw >= -50)
                {
                    _trend = SgvTrend.Flat;
                }

                if (RateOfChangeRaw <= -51 && RateOfChangeRaw >= -100)
                {
                    _trend = SgvTrend.FortyFiveDown;
                }
                if (RateOfChangeRaw <= -101 && RateOfChangeRaw >= -300)
                {
                    _trend = SgvTrend.SingleDown;
                }
                //maybe there is a max
                if (RateOfChangeRaw < -300)
                {
                    _trend = SgvTrend.DoubleDown;
                }
            }
            return _trend;
        }

        private void BgReadings()
        {
            var bGReadings = eventsToHandle.Where(e => e.EventType == EventTypeEnum.BG_READING)
                .Where(e => ((BG_READING_Event)e.Message).BgSource == BgSourceEnum.EXTERNAL_METER && (((BG_READING_Event)e.Message).BgUnits == BgUnitEnum.MMOL_L || ((BG_READING_Event)e.Message).BgUnits == BgUnitEnum.MG_DL)).ToList();

            foreach (var item in bGReadings)
            {
                CreateBgReading((BG_READING_Event)item.Message, item.Key);
            }
        }

        protected void CreateEntrySgv(int sgvValue, string dateString, double epoch, string direction, bool checkIfExists, string key, ushort prediction, double isig)
        {

            string serialNum = _session.SessionDevice.Device.SerialNumberFull;
            Entry entry = new Entry();
            entry.Date = epoch;
            entry.Direction = direction;
            entry.Sgv = sgvValue;
            entry.Type = "sgv";
            entry.DateString = dateString;
            entry.Device = string.Format("medtronic-640g://{0}", serialNum);
            entry.Key = key;
            entry.Isig = isig;

            // entry.Noise
            if (entry.Sgv <= 400)
            {
                this.Entries.Add(entry);
                //if (checkIfExists)
                //{
                //    var last = await _client.EntriesAsync(null, 1);
                //    if (last.Count == 0 || !entry.DateString.Equals(last[0].DateString))
                //    {
                //        this.Entries.Add(entry);
                //    }
                //    else
                //    {
                //        Logger.LogInformation("Entry not uploaded to Nightscout. Already exists.");
                //    }
                //}
                //else
                //{
                //    this.Entries.Add(entry);
                //}
            }
            else
            {
                if (NightscoutConfiguration.HandleAlert776)
                {
                    switch (entry.Sgv)
                    {
                        case 776:
                            entry.Sgv = 401;
                            this.Entries.Add(entry);
                            break;
                        case 777:
                            entry.Sgv = 39;
                            this.Entries.Add(entry);
                            break;
                        default:
                            CreateSgvEntryAlert(entry);
                            break;
                    }
                    //if (entry.Sgv == 776)
                    //{
                    //    //this is done to match the pumps diagram. Testing.....
                    //    entry.Sgv = 400;
                    //    this.Entries.Add(entry);
                    //}
                    //else
                    //{
                    //    CreateSgvEntryAlert(entry);
                    //}
                    //if (entry.Sgv==777)
                    //{
                    //    entry.Sgv = 40;
                    //    this.Entries.Add(entry);
                    //}
                }
                else
                {
                    CreateSgvEntryAlert(entry);
                }

            }




        }

        private void CreateDeviceStatus()
        {

            PumpStatusMessage message = this.LastStatusMessage;
            string serialNum = _session.SessionDevice.Device.SerialNumberFull;
            DateTime create = _session.PumpTime.PumpDateTime.Value;

            this.DeviceStatus = new DeviceStatus();

            this.DeviceStatus.UploaderBattery = _session.DataLoggerBattery;
            this.DeviceStatus.Device = string.Format("medtronic-640g://{0}", serialNum);
            this.DeviceStatus.CreatedAt = create.ToString(Constants.Dateformat);
            this.DeviceStatus.PumpInfo.Reservoir = Math.Round(message.ReservoirAmount, 3);
            this.DeviceStatus.PumpInfo.Iob.Bolusiob = Math.Round(message.ActiveInsulin.Insulin, 3);
            //this.DeviceStatus.PumpInfo.Iob.Timestamp = create.ToString("ddd, MMM dd HH:mm:ss CEST yyyy");
            this.DeviceStatus.PumpInfo.Iob.Timestamp = create.ToString(Constants.Dateformat);
            this.DeviceStatus.PumpInfo.Clock = create.ToString(Constants.Dateformat);
            this.DeviceStatus.PumpInfo.Battery.Percent = message.BatteryPercentage;



            this.DeviceStatus.PumpInfo.Status.Add("supended", false);
            this.DeviceStatus.PumpInfo.Status.Add("bolusing", false);

            //build status message
            string statusMessage = "";

            if (message.Status.Suspended)
            {
                statusMessage = "suspended";
            }
            if (message.Status.BolusingNormal)
            {
                statusMessage = "bolusing";
            }

            if (!message.Status.Suspended && !message.Status.BolusingNormal)
            {
                statusMessage = "normal";
            }
            if (message.SensorCalibrationDateTime.HasValue)
            {
                var diff = message.SensorCalibrationDateTime.Value.Subtract(DateTime.Now);
                statusMessage += $" - Cal.{diff.Hours}h{diff.Minutes}m ({message.SensorCalibrationDateTime.Value.ToString("HH:mm")})";
                //statusMessage += $" - Cal. {message.SensorCalibrationDateTime.Value.ToString()}";
            }

            if (message.SensorStatus.Calibrating)
            {
                statusMessage += " - Calibrating....";
            }


            this.DeviceStatus.PumpInfo.Status.Add("status", statusMessage);

        }

        private void CreateCorrectionBolus(double insulin, double carbs, string reference, string dateTime, string key)
        {
            Treatment treatment = new Treatment();
            treatment.Insulin = insulin;// message.BolusEstimate;
            treatment.Carbs = carbs;
            if (treatment.Carbs.HasValue && treatment.Carbs.Value != 0)
            {
                treatment.EventType = "Meal Bolus";
            }
            else
            {
                treatment.EventType = "Correction Bolus";
            }
            treatment.Key = key;
            //treatment.EnteredBy = $"Ref:{reference}";// $"Ref:{message.LastBolusReference}";
            treatment.Created_at = dateTime;// message.LastBolusDateTime.ToString(dateformat);
            Treatments.Add(treatment);

            //var last = await _client.TreatmentsAsync($"find[eventType]={treatment.EventType.Replace(" ", "+")}&find[enteredBy]={treatment.EnteredBy}", 1);
            //if (last.Count == 0)
            //{

            //}
        }

        private void CreateNote(string note, DateTime createdAt, string key)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Note";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat); ;
            treatment.Notes = note;
            treatment.Key = key;
            Treatments.Add(treatment);

        }

        private void CreateBattery(string note, DateTime createdAt, string key)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Pump Battery Change";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat); ;
            treatment.Notes = note;
            treatment.Key = key;
            Treatments.Add(treatment);

        }

        private void CreateExercise(string note, DateTime createdAt, string key, Int16 duration)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Exercise";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            treatment.Duration = duration.ToString();
            treatment.Notes = note;
            treatment.Key = key;

            Treatments.Add(treatment);

        }

        private string GetHtmlTable(DAILY_TOTALS_Event dtEvent)
        {
            string table = "";
            Dictionary<string, string> sg = new Dictionary<string, string>();
            sg.Add("Average", $"{dtEvent.BG_AVERAGE}/{dtEvent.SG_AVERAGE}");
            //sg.Add("High", $"{dtEvent.HIGH_METER_BG}/{dtEvent.sg}");

            return table;
        }

        private void CreateDailyTotal(DAILY_TOTALS_Event dtEvent, DateTime createdAt, string key)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Note";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat); ;
            treatment.Notes = "Daily Totals<br>" + GetHtmlTable(dtEvent);
            treatment.Key = key;
            treatment.Insulin = 22.3; //place on the top of the graph
            Treatments.Add(treatment);

        }

        private void CreateSgvEntryAlert(Entry entry)
        {
            if (entry.Sgv.HasValue)
            {
                var alert = (SgvAlert)entry.Sgv.Value;

                var date = entry.Date.Value.GetDateTime();
                //var note = $"{alert.ToString()} - ({entry.DateString})";
                string msg = $"{alert.ToString()}";
                if (entry.Isig.HasValue)
                {
                    msg = $"{alert.ToString()} <br> ISIG:{entry.Isig.ToString()}";
                }
                //var note = $"{alert.ToString()}";
                var treatment = CreateAnnouncement(msg, date, "Alert", entry.Key);

            }

        }

        private Treatment CreateAnnouncement(string note, DateTime createdAt, string reference, string key)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Announcement";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            treatment.Notes = note;
            treatment.Key = key;

            //treatment.EnteredBy = $"ref:${reference}";
            Treatments.Add(treatment);
            return treatment;

        }

        private Treatment CreateCgmSensorChanged(DateTime createdAt, string key)
        {

            Treatment treatment = new Treatment();
            treatment.EventType = "Sensor Change";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            //treatment.EnteredBy = $"ref:${treatment.EventType}";
            treatment.Key = key;
            Treatments.Add(treatment);



            Treatment treatment2 = new Treatment();
            treatment2.EventType = "Sensor Start";
            treatment2.Created_at = createdAt.ToString(Constants.Dateformat);
            //treatment2.EnteredBy = $"ref:${treatment.EventType}";
            treatment2.Key = key;
            Treatments.Add(treatment2);

            return treatment;

        }

        private Treatment CreateTempBasalProgrammed(DateTime createdAt, string key)
        {


            Treatment treatment = new Treatment();
            treatment.EventType = "Temp Basal";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            ////treatment.EnteredBy = $"ref:${treatment.EventType}";
            treatment.Key = key;
            Treatments.Add(treatment);

            return treatment;
        }

        private Treatment CreateInsulinChanged(DateTime createdAt, string key)
        {
            Treatment treatment = new Treatment();
            treatment.EventType = "Insulin Change";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            ////treatment.EnteredBy = $"ref:${treatment.EventType}";
            treatment.Key = key;
            Treatments.Add(treatment);

            return treatment;
        }

        private Treatment CreateCannulaChanged(DateTime createdAt, string key)
        {

            Treatment treatment = new Treatment();
            treatment.EventType = "Site Change";
            treatment.Created_at = createdAt.ToString(Constants.Dateformat);
            //treatment.EnteredBy = $"ref:${treatment.EventType}";
            treatment.Key = key;
            Treatments.Add(treatment);

            return treatment;
        }

        private Treatment CreateBgReading(BG_READING_Event bgEvent, string key)
        {

            Treatment treatment = new Treatment();
            treatment.EventType = "BG Check";
            treatment.GlucoseType = "Finger";
            treatment.Key = key;
            if (bgEvent.BgUnits == BgUnitEnum.MMOL_L)
            {
                treatment.Glucose = bgEvent.BgValueInMmol.ToString();
                treatment.Units = "mmol";
            }
            else
            {
                treatment.Glucose = bgEvent.BgValueInMg.ToString();
                treatment.Units = "mg/dl";
            }


            treatment.Created_at = bgEvent.EventDate.DateTime.Value.ToString(Constants.Dateformat);
            //treatment.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment);

            return treatment;
        }

    }
}
