using CGM.Communication.Common.Serialize;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Data.Mongo
{
    public class MongoUnitOfWork
    {
        private string _connectionString;
        private MongoClient _client;
        private IMongoDatabase _db;
        private string eventCollectionName = "600PumpEvent";
        protected ILogger Logger = ApplicationLogging.CreateLogger<MongoUnitOfWork>();

        public MongoUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _client = new MongoClient(_connectionString);
            var split = connectionString.Split('/');
            _db = _client.GetDatabase(split[3]);

        }

        public void Insert<T>(string collectionName,List<T> list)
        {
            var coll = _db.GetCollection<T>(collectionName);
            coll.InsertMany(list);
        }
        public void Insert<T>(string collectionName, T entity)
        {
            var coll = _db.GetCollection<T>(collectionName);
            coll.InsertOne(entity);
        }

        public void SavePumpSettings(PumpSettings pumpSettings)
        {
            var coll = _db.GetCollection<PumpSettings>("600Pumpsettings");
            coll.DeleteOne<PumpSettings>(e => e.Id == 1);
            coll.InsertOne(pumpSettings);
        }

        public void SaveSession(SerializerSession session)
        {

            if (session != null && session.PumpDataHistory != null && (session.PumpDataHistory.PumpEvents.Count > 0 || session.PumpDataHistory.SensorEvents.Count > 0))
            {
                LastInserted last = GetLastInserted();

                if (session.PumpDataHistory.PumpEvents.Count > 0)
                {
                   

                    //Pump events
                    var filteredpumpevents = session.PumpDataHistory.PumpEvents.Where(e =>
                            e.EventType != EventTypeEnum.PLGM_CONTROLLER_STATE).ToList();

                
                    if (last.Pump.Rtc!=0)
                    {
                        filteredpumpevents = filteredpumpevents.Where(e => e.Rtc > last.Pump.Rtc).ToList();
                    }
                    if (filteredpumpevents.Count > 0)
                    {
                        //filteredpumpevents.ForEach(e => e.Index = 0);
                        //filteredpumpevents.Last().Index = 1;
                        this.InsertPumpEvent(filteredpumpevents, HistoryDataTypeEnum.Pump);
                        session.PumpDataHistory.PumpEventsNew = filteredpumpevents;
                        Logger.LogInformation($"{filteredpumpevents.Count} pumpevents uploaded to MongoDb");
                        last.Pump.Rtc = filteredpumpevents.Last().Rtc;
                    }

                }

                if (session.PumpDataHistory.SensorEvents.Count > 0)
                {
                    //sensor events
                    var filteredsensorevents = session.PumpDataHistory.SensorEvents.Where(e =>
                           e.EventType != EventTypeEnum.PLGM_CONTROLLER_STATE
                           ).ToList();


                    if (last.Sensor.Rtc != 0)
                    {
                        filteredsensorevents = filteredsensorevents.Where(e => e.Rtc > last.Sensor.Rtc).ToList();
                    }
                    if (filteredsensorevents.Count > 0)
                    {
                        //filteredsensorevents.ForEach(e => e.Index = 0);
                        //filteredsensorevents.Last().Index = 1;
                         this.InsertPumpEvent(filteredsensorevents, HistoryDataTypeEnum.Sensor);
                        session.PumpDataHistory.SensorEventsNew = filteredsensorevents;
                        Logger.LogInformation($"{filteredsensorevents.Count} sensorevents uploaded to MongoDb");
                        last.Sensor.Rtc = filteredsensorevents.Last().Rtc;

                        var readings = filteredsensorevents.Where(e => e.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED).Select(e => (SENSOR_GLUCOSE_READINGS_EXTENDED_Event)e.Message);
                        var details = readings.SelectMany(e => e.Details).ToList();

                         this.InsertSensorData(details);
                        Logger.LogInformation($"{details.Count} sensordetails uploaded to MongoDb");
                    }

                }

                if (string.IsNullOrEmpty(last.PumpSettings) || session.PumpSettings.IsNew(last.PumpSettings))
                {
                    last.PumpSettings = session.PumpSettings.GetCompareString();
                    SavePumpSettings(session.PumpSettings);

                }

                List<PumpStatusMessage> statusList= session.Status;
                if (last.StatusRtc!=0)
                {
                    statusList = session.Status.Where(e => e.SgvDateTime.Rtc > last.StatusRtc).ToList();
                }
                if (statusList.Count>0)
                {
                    this.Insert<PumpStatusMessage>("600PumpStatus", statusList);
                    last.StatusRtc = statusList.Last().SgvDateTime.Rtc;
                }
                LastInserted(last);


                NightscoutMongoUploader uploader = new NightscoutMongoUploader(session);
                CancellationTokenSource _tokenSource = new CancellationTokenSource();
                CancellationToken _token = _tokenSource.Token;
                uploader.Upload(_token);


            }
            else
            {
                Logger.LogInformation("No data uploaded to MongoDb");
            }
            //await ReplaceOne(session.Device);
        }

        public void DeleteCollections()
        {
            _db.DropCollection("devicestatus");
            _db.DropCollection("entries");
            _db.DropCollection("treatments");
            _db.DropCollection("600GlucoseReadingsDetail");
            _db.DropCollection("600LastInserted");
            _db.DropCollection("600PumpEventPump");
            _db.DropCollection("600PumpEventSensor");
            _db.DropCollection("600Pumpsettings");
            _db.DropCollection("600PumpStatus");
            
        }

        private IMongoCollection<PumpEvent> GetCollection(HistoryDataTypeEnum historyDataType)
        {
            string collectionname = $"{eventCollectionName}{historyDataType.ToString()}";
            var coll = _db.GetCollection<PumpEvent>(collectionname);
            return coll;
        }

        private void UpdateOne(PumpEvent pumpEvent)
        {
            string collectionname = $"{eventCollectionName}{pumpEvent.HistoryDataType.ToString()}";
            var coll = _db.GetCollection<PumpEvent>(collectionname);

            var filter = Builders<PumpEvent>.Filter.Eq(e=>e.BytesAsString, pumpEvent.BytesAsString);
            var update = Builders<PumpEvent>.Update.Set(e=>e.Index, 0);

 
            coll.UpdateOne(filter, update);

        }

        private LastInserted GetLastInserted()
        {
            string collectionname = "600LastInserted";
            var coll = _db.GetCollection<LastInserted>(collectionname);
            var filter = Builders<LastInserted>
                 .Filter
                 .Eq(x => x.Key, 1);
            Mongo.LastInserted lin = coll.Find(filter).FirstOrDefault() ;
            if (lin==null)
            {
                lin= new Mongo.LastInserted();
                coll.InsertOne(lin);
            }
            
            return lin;

        }

        private void LastInserted(LastInserted last)
        {
            string collectionname = "600LastInserted";
            var coll = _db.GetCollection<LastInserted>(collectionname);
            var filter = Builders<LastInserted>
                 .Filter
                 .Eq(x => x.Key, 1);
            coll.ReplaceOne(filter, last);

        }

        private void ReplaceOne(BayerStickInfoResponse device)
        {
            string collectionname = "600BayerStickInfo";
            var coll = _db.GetCollection<BayerStickInfoResponse>(collectionname);
            var filter = Builders<BayerStickInfoResponse>
                 .Filter
                 .Eq(x => x.SerialNumberFull, device.SerialNumber);
            coll.ReplaceOne(filter, device);

        }

        private void InsertSensorData(List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> events)
        {
            string collectionname = "600GlucoseReadingsDetail";
            var coll = _db.GetCollection<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>(collectionname);
            coll.InsertMany(events);
        }

        private void InsertPumpEvent(List<PumpEvent> events, HistoryDataTypeEnum historyDataType)
        {
            var coll = GetCollection(historyDataType);
            try
            {
                // coll.UpdateMany( filter, events, new UpdateOptions { IsUpsert = true });
                coll.InsertMany(events);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


    }
}
