using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses.Events;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Data.Mongo
{
    public class MongoUnitOfWork
    {
        private string _connectionString;
        private MongoClient _client;
        private IMongoDatabase _db;

        public MongoUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _client = new MongoClient(_connectionString);
            var split = connectionString.Split('/');
            _db = _client.GetDatabase(split[3]);

        }

        public async Task SaveSession(SerializerSession session)
        {
            
        }

        private async Task InsertGlucoseReadingsDetail(List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> events)
        {
            string collectionname = "600GlucoseReadingsDetail";
            var coll=_db.GetCollection<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>(collectionname);
            coll.InsertMany(events);
        }


        private async Task InsertSensorData2(SENSOR_GLUCOSE_READINGS_EXTENDED_Event events)
        {
            string collectionname = "600GlucoseReadings";
            var coll = _db.GetCollection<SENSOR_GLUCOSE_READINGS_EXTENDED_Event>(collectionname);
            //if (coll==null)
            //{
            //   await _db.CreateCollectionAsync(collectionname);
            //}

            coll.InsertOne(events);


        }

        private async Task InsertPumpEvent(List<PumpEvent> events, HistoryDataTypeEnum historyDataType)
        {
            string collectionname = $"600PumpEvent{historyDataType.ToString()}";
            //_db.DropCollection(collectionname);
            var coll = _db.GetCollection<PumpEvent>(collectionname);
          await  coll.InsertManyAsync(events);
        }


    }
}
