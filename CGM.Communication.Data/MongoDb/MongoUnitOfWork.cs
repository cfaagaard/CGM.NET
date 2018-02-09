using CGM.Communication.MiniMed.Responses.Events;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Data.MongoDb
{
    public class MongoUnitOfWork : IDisposable
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

        public async Task InsertSensorData(List<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail> events)
        {
            string collectionname = "600SensorData";
            var coll=_db.GetCollection<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>(collectionname);
            //if (coll==null)
            //{
            //   await _db.CreateCollectionAsync(collectionname);
            //}

            coll.InsertMany(events);


        }


        public async Task InsertSensorData2(SENSOR_GLUCOSE_READINGS_EXTENDED_Event events)
        {
            string collectionname = "600SensorDataEvent";
            var coll = _db.GetCollection<SENSOR_GLUCOSE_READINGS_EXTENDED_Event>(collectionname);
            //if (coll==null)
            //{
            //   await _db.CreateCollectionAsync(collectionname);
            //}

            coll.InsertOne(events);


        }

        public async Task InsertPumpEvent(List<PumpEvent> events)
        {
            string collectionname = "600SensorPumpEvent";
            //_db.DropCollection(collectionname);
            var coll = _db.GetCollection<PumpEvent>(collectionname);

           
            coll.InsertMany(events);


        }

        public void Dispose()
        {
            //maybe not dispose
        }
    }
}
