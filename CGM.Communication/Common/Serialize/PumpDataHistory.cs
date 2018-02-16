using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Requests;
using CGM.Communication.MiniMed.Infrastructur;
using System.Threading.Tasks;
using CGM.Communication.MiniMed;

namespace CGM.Communication.Common.Serialize
{
    public class PumpDataHistory
    {
        private SerializerSession _session;

        public List<PumpEvent> PumpEvents { get; set; } = new List<PumpEvent>();
        public List<PumpEvent> SensorEvents { get; set; } = new List<PumpEvent>();

        public List<PumpEvent> PumpEventsNew { get; set; } = new List<PumpEvent>();
        public List<PumpEvent> SensorEventsNew { get; set; } = new List<PumpEvent>();

        internal MultiPacketHandler CurrentMultiPacketHandler { get; set; }


        internal List<MultiPacketHandler> MultiPacketHandlers { get; set; } = new List<MultiPacketHandler>();

        //public DateTime? From { get; set; }
        //public DateTime? To { get; set; }

        public PumpDataHistory(SerializerSession session)
        {
            _session = session;

        }



        public void AddMultiHandler(PumpStateHistoryReadInfoResponse response)
        {
            MultiPacketHandler handler = new MultiPacketHandler(_session);
            handler.ReadInfoResponse = response;
            MultiPacketHandlers.Add(handler);
            _session.SessionVariables.GetNextMultiPacketIndex();
            //CurrentMultiPacketHandler = handler;
        }

        public int GetSize(HistoryDataTypeEnum historyDataType)
        {
            var handler = MultiPacketHandlers.FirstOrDefault(e => e.ReadInfoResponse.HistoryDataType == (HistoryDataTypeEnum)historyDataType);// MultiPacketHandlers.FirstOrDefault(e => e.ReadInfoResponse.HistoryDataType == (HistoryDataTypeEnum)historyDataType);
            if (handler != null)
            {
                return handler.ReadInfoResponse.HistorySize;
            }
            return 0;
        }

        public void SetCurrentMulitPacket(ReadHistoryRequest request)
        {
            if (this.MultiPacketHandlers.Count== 0)
            {
                throw new Exception("MultipacketHandler not set.");
            }

            var packet = this.MultiPacketHandlers.FirstOrDefault(e => e.ReadInfoResponse.HistoryDataType == request.HistoryDataType);
            if (packet == null)
            {
                throw new Exception($"Could not find MultipacketHandler for {request.HistoryDataType}");
            }
            this.CurrentMultiPacketHandler = packet;// this.MultiPacketHandlers[_session.SessionVariables.GetCurrentMultiPacketIndex()]; // MultiPacketHandlers.FirstOrDefault(e => e.ReadInfoResponse.HistoryDataType == (HistoryDataTypeEnum)request.HistoryDataType);
        }

        internal void ExtractHistoryEvents()
        {
            if (MultiPacketHandlers!=null && MultiPacketHandlers.Count>0)
            {
                foreach (var item in MultiPacketHandlers)
                {
                    item.GetHistoryEvents();
                    var events=item.JoinAllEvents();
                    switch (item.ReadInfoResponse.HistoryDataType)
                    {
                        
                        case HistoryDataTypeEnum.Pump:
                            this.PumpEvents.AddRange(events);
                            break;
                        case HistoryDataTypeEnum.Sensor:
                            this.SensorEvents.AddRange(events);
                            break;
                        default:
                            break;
                    }
                }

                //MultiPacketHandlers.ForEach(e => e.GetHistoryEvents());
            }
            
        }

        public override string ToString()
        {
            if (this.CurrentMultiPacketHandler.ReadInfoResponse != null)
            {
                return $"{this.CurrentMultiPacketHandler.ReadInfoResponse.ToString()} ()"; //{this.CurrentMultiPacketHandler.PumpStateHistory.Count}
            }
            return "(No dates)";
        }

        //public List<PumpEvent> JoinAllEvents()
        //{
        //    List<PumpEvent> all = new List<PumpEvent>();
        //    MultiPacketHandlers.ForEach(e => all.AddRange(e.JoinAllEvents()));
        //    return all.OrderBy(e => e.Timestamp).ToList(); ;
        //}
    }
}
