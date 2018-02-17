using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Responses.Events;
using CGM.Data.Nightscout.RestApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CGM.Data.Mongo
{
    public class NightscoutMongoUploader : UploadLogic
    {
        MongoUnitOfWork _uow;
        SerializerSession _session;
        public NightscoutMongoUploader(SerializerSession session) : base(session)
        {
            _session = session;
            _uow = new MongoUnitOfWork(session.Settings.MongoDbUrl);
        }

        protected override void AddDeviceStatus(CancellationToken cancelToken)
        {
            _uow.Insert<Nightscout.RestApi.DeviceStatus>("devicestatus", this.DeviceStatus);
        }

        protected override void AddEntries(CancellationToken cancelToken)
        {
            _uow.Insert<Nightscout.RestApi.Entry>("entries", this.Entries);
        }

        protected override void AddTreatments(CancellationToken cancelToken)
        {
            _uow.Insert<Nightscout.RestApi.Treatment>("treatments", this.Treatments);
        }

        protected override List<PumpEvent> GetHistoryWithNoStatus(List<int> eventFilter)
        {
            List<PumpEvent> events = new List<PumpEvent>(Session.PumpDataHistory.PumpEventsNew);
            events.AddRange(Session.PumpDataHistory.SensorEventsNew);

            return events;
        }
    }
}
