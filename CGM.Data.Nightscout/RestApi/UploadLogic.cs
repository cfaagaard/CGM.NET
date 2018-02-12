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


namespace CGM.Data.Nightscout.RestApi
{



    public abstract class UploadLogic
    {

        private string dateformat = "yyyy-MM-ddTHH\\:mm\\:sszzz";
        protected ILogger Logger = ApplicationLogging.CreateLogger<UploadLogic>();

        private SerializerSession _session;
        protected NightscoutClient _client;

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
            if (string.IsNullOrEmpty(_session.Settings.NightscoutApiUrl) || string.IsNullOrEmpty(_session.Settings.NightscoutSecretkey))
            {
                throw new ArgumentException("Nightscout url or apikey is null.");
            }
            _client = new NightscoutClient(_session.Settings.NightscoutApiUrl, _session.Settings.NightscoutSecretkey);
        }

        protected abstract List<PumpEvent> GetHistoryWithNoStatus(List<int> eventFilter);
        protected abstract void AddEntries(CancellationToken cancelToken);
        protected abstract void AddTreatments(CancellationToken cancelToken);
        protected abstract void AddDeviceStatus(CancellationToken cancelToken);

        public async Task Upload(CancellationToken cancelToken)
        {

            List<int> eventFilter = new List<int>();
            eventFilter.Add((int)EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED);
            eventFilter.Add((int)EventTypeEnum.BOLUS_WIZARD_ESTIMATE);
            eventFilter.Add((int)EventTypeEnum.ALARM_NOTIFICATION);
            eventFilter.Add((int)EventTypeEnum.GLUCOSE_SENSOR_CHANGE);
            eventFilter.Add((int)EventTypeEnum.CANNULA_FILL_DELIVERED);
            eventFilter.Add((int)EventTypeEnum.BG_READING);
            eventFilter.Add((int)EventTypeEnum.AIRPLANE_MODE);
            eventFilter.Add((int)EventTypeEnum.TEMP_BASAL_PROGRAMMED);

            List<PumpEvent> eventsToHandle = GetHistoryWithNoStatus(eventFilter);

            CreateDeviceStatus();
            AddDeviceStatus(cancelToken);

            if (eventsToHandle.Count > 0)
            {
                MissingReadings(eventsToHandle);
                MissingWizard(eventsToHandle);
                MissingAlerts(eventsToHandle);
                MissingTempBasalProgrammed(eventsToHandle);
                SensorChange(eventsToHandle);
                CannulaChanged(eventsToHandle);
                BgReadings(eventsToHandle);
                OtherReadings(eventsToHandle);

                AddEntries(cancelToken);
                AddTreatments(cancelToken);
               

                await Notify(cancelToken);

            }
            else
            {
                Logger.LogInformation($"No new pump-events.");
            }
        }

        private void MissingTempBasalProgrammed(IEnumerable<PumpEvent> allEvents)
        {
            var events = allEvents.Where(e => e.EventType == EventTypeEnum.TEMP_BASAL_PROGRAMMED);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {

                    var treatment = CreateTempBasalProgrammed(item.Timestamp.Value, item.Key);
                    var msg = (TEMP_BASAL_PROGRAMMED_Event)item.Message;

                    treatment.Duration = msg.Duration.ToString();
                    treatment.Percent = msg.Percentage.ToString();
                }
            }

        }

        private void OtherReadings(List<PumpEvent> eventsToHandle)
        {
            var events = eventsToHandle.Where(e => e.EventType == EventTypeEnum.AIRPLANE_MODE);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    string status = "On";
                    if (item.Message.AllBytes[0] == 0x00)
                    {
                        status = "Off";
                    }
                    CreateNote(string.Format("{0} ({1})", EventTypeEnum.AIRPLANE_MODE.ToString(), status), item.Timestamp.Value, item.Key);

                }
            }
        }

        private async Task Notify(CancellationToken cancelToken)
        {
            if (!string.IsNullOrEmpty(Session.Settings.NotificationUrl) && Session.Settings.SendEventsToNotificationUrl)
            {
                var notif = this.Treatments.Where(e => !string.IsNullOrEmpty(e.Notification.Type)).Select(e => e.Notification);
                if (notif.Count() > 0)
                {
                    NotificationClient client = new NotificationClient(Session.Settings.NotificationUrl);
                    foreach (var item in notif)
                    {
                        await client.AddNotificationAsync(item, cancelToken);
                    }
                    Logger.LogInformation($"Notifications sent. ({notif.Count()})");
                }

            }
        }

        //private async Task UploadElements_old(CancellationToken cancelToken)
        //{
        //    if (Entries.Count > 0)
        //    {
        //        try
        //        {

        //            await _client.AddEntriesAsync(Entries, cancelToken);
        //            Logger.LogInformation($"Entries uploaded to Nightscout. ({Entries.Count})");
        //            //log uploads
        //            using (CgmUnitOfWork uow = new CgmUnitOfWork())
        //            {
        //                uow.HistoryStatus.AddKeys(Entries.Select(e => e.Key).ToList(), HistoryStatusTypeEnum.NightScout, 0);
        //            }

        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }



        //    if (this.Treatments.Count > 0)
        //    {
        //        try
        //        {
        //            await _client.AddTreatmentsAsync(this.Treatments, cancelToken);
        //            Logger.LogInformation($"Treatments uploaded to Nightscout. ({Treatments.Count})");
        //            using (CgmUnitOfWork uow = new CgmUnitOfWork())
        //            {
        //                uow.HistoryStatus.AddKeys(Treatments.Select(e => e.Key).ToList(), HistoryStatusTypeEnum.NightScout, 0);
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }

        //    if (this.DeviceStatus != null && !string.IsNullOrEmpty(this.DeviceStatus.Device))
        //    {
        //        await _client.AddDeviceStatusAsync(new List<DeviceStatus>() { this.DeviceStatus }, cancelToken);
        //        Logger.LogInformation("DeviceStatus uploaded to Nightscout.");
        //    }
        //}

        private void SensorChange(IEnumerable<PumpEvent> allEvents)
        {
            var events = allEvents.Where(e => e.EventType == EventTypeEnum.GLUCOSE_SENSOR_CHANGE);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    var treatment = CreateCgmSensorChanged(item.Timestamp.Value, item.Key);
                    treatment.Notification.Date = item.Timestamp.Value.ToString();
                    treatment.Notification.Type = EventTypeEnum.GLUCOSE_SENSOR_CHANGE.ToString();
                    treatment.Notification.Text = EventTypeEnum.GLUCOSE_SENSOR_CHANGE.ToString();
                }
            }
        }

        private void CannulaChanged(IEnumerable<PumpEvent> allEvents)
        {
            //is this correct?
            var events = allEvents.Where(e => e.EventType == EventTypeEnum.CANNULA_FILL_DELIVERED && ((CANNULA_FILL_DELIVERED_Event)e.Message).PRIME_TYPE_NAME == PrimeTypeEnum.canulla_fill);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    var treatment = CreateInsulinChanged(item.Timestamp.Value, item.Key);
                    treatment.Notification.Date = item.Timestamp.Value.ToString();
                    treatment.Notification.Type = EventTypeEnum.CANNULA_FILL_DELIVERED.ToString();
                    treatment.Notification.Text = EventTypeEnum.CANNULA_FILL_DELIVERED.ToString();
                }
            }
        }

        private void MissingAlerts(IEnumerable<PumpEvent> allEvents)
        {
            var events = allEvents.Where(e => e.EventType == EventTypeEnum.ALARM_NOTIFICATION);
            int count = events.Count();
            if (count > 0)
            {
                foreach (var item in events)
                {
                    var msg = (ALARM_NOTIFICATION_Event)item.Message;

                    var announcement = CreateAnnouncement($"{msg.AlarmTypeName.ToString()}", msg.Timestamp.Value, "Alert", item.Key);
                    //announcement.Notification.Date = msg.Timestamp.Value.ToString();
                    //announcement.Notification.Type = EventTypeEnum.ALARM_NOTIFICATION.ToString();
                    //announcement.Notification.Text = msg.AlarmTypeName.ToString();

                    if (msg.AlarmTypeName == Alerts.Suspend_Before_Low_Alarm_quiet_810
                        || msg.AlarmTypeName == Alerts.Suspend_Before_Low_Alarm_811
                         || msg.AlarmTypeName == Alerts.Suspend_On_Low_Alarm_809
                        )
                    {
                        var treatment = CreateTempBasalProgrammed(item.Timestamp.Value, item.Key);

                        treatment.Duration = "60";
                        treatment.absolute = "0";
                    }

                    if (msg.AlarmTypeName == Alerts.Basal_Delivery_Resumed_Alert_quiet_806
                        || msg.AlarmTypeName == Alerts.Basal_Delivery_Resumed_Alert_glucose_still_low_maximum_suspend_reached_814
                        || msg.AlarmTypeName == Alerts.Basal_Delivery_Resumed_Alert_maximum_suspend_reached_808)
                    {
                        var treatment = CreateTempBasalProgrammed(item.Timestamp.Value, item.Key);

                        treatment.Duration = "0";
                    }
                }
            }
        }

        private void MissingWizard(List<PumpEvent> allEvents)
        {
            //BOLUS_WIZARD_ESTIMATE_Event
            var wizards = allEvents.Where(e => e.EventType == EventTypeEnum.BOLUS_WIZARD_ESTIMATE).ToList();
            int count = wizards.Count();
            if (count > 0)
            {
                foreach (var item in wizards)
                {
                    var msg = (BOLUS_WIZARD_ESTIMATE_Event)item.Message;
                    if (msg.FinalEstimate.INSULIN != 0 || msg.CARB_INPUT.CARB != 0)
                    {
                        CreateCorrectionBolus(msg.FinalEstimate.INSULIN, msg.CARB_INPUT.CARB, item.Rtc.ToString(), item.Timestamp.Value.ToString(Constants.Dateformat), item.Key);
                    }
                    allEvents.Remove(item);
                }
            }
        }

        private void MissingReadings(List<PumpEvent> allEvents)
        {

            var sensorReadings = allEvents.Where(e => e.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED).ToList();

            if (sensorReadings.Count > 0)
            {
                List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> details = new List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>();

                foreach (var msg in sensorReadings)
                {

                    var reading = (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)msg.Message;

                    for (int i = 0; i < reading.Details.Count; i++)
                    {
                        var read = reading.Details[i];

                        CreateEntrySgv(read.Amount, read.Timestamp.Value.ToString(dateformat), read.Epoch, read.Trend.ToString(), false, msg.Key, read.PredictedSg, read.Isig);
                        //if (reading.Timestamp.HasValue)
                        //{
                        //    var read = reading.Details[i];
                        //var readingRtc = msg.Rtc - (i * reading.MinutesBetweenReadings * 60);
                        //read.Timestamp = DateTimeExtension.GetDateTime(readingRtc, msg.Offset);// reading.Timestamp.Value.AddMinutes((i * reading.MinutesBetweenReadings*-1));
                        //    CreateEntrySgv(read.Amount, read.Timestamp.Value.ToString(dateformat), read.Epoch, read.Trend.ToString(), false, msg.Key);

                        //}
                    }
                    allEvents.Remove(msg);
                }
            }
        }

        private void BgReadings(IEnumerable<PumpEvent> allEvents)
        {
            var bGReadings = allEvents.Where(e => e.EventType == EventTypeEnum.BG_READING)
                .Where(e => ((BG_READING_Event)e.Message).BgSource == BgSourceEnum.EXTERNAL_METER && (((BG_READING_Event)e.Message).BgUnits == BgUnitEnum.MMOL_L || ((BG_READING_Event)e.Message).BgUnits == BgUnitEnum.MG_DL)).ToList();

            if (bGReadings.Count > 0)
            {
                foreach (var item in bGReadings)
                {
                    //.Select(e => (BG_READING_Event)e.Message)
                    CreateBgReading((BG_READING_Event)item.Message, item.Key);
                }
            }
        }

        protected void CreateEntrySgv(int sgvValue, string dateString, long epoch, string direction, bool checkIfExists, string key, ushort prediction, double isig)
        {

            string serialNum = _session.Device.SerialNumberFull;
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
                if (Session.Settings.HandleAlert776)
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
            string serialNum = _session.Device.SerialNumberFull;
            DateTime create = _session.PumpTime.PumpDateTime.Value;

            this.DeviceStatus = new DeviceStatus();

            this.DeviceStatus.UploaderBattery = _session.UploaderBattery;
            this.DeviceStatus.Device = string.Format("medtronic-640g://{0}", serialNum);
            this.DeviceStatus.CreatedAt = create.ToString(Constants.Dateformat);
            this.DeviceStatus.PumpInfo.Reservoir = Math.Round(message.ReservoirAmount, 3);
            this.DeviceStatus.PumpInfo.Iob.Bolusiob = Math.Round(message.ActiveInsulin.INSULIN, 3);
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
                CreateAnnouncement(msg, date, "Alert", entry.Key);

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


            treatment.Created_at = bgEvent.Timestamp.Value.ToString(Constants.Dateformat);
            //treatment.EnteredBy = $"ref:${treatment.EventType} - {treatment.Created_at}";
            Treatments.Add(treatment);

            return treatment;
        }

    }
}
