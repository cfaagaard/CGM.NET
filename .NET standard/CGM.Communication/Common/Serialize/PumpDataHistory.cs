using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    public class PumpDataHistory
    {
        private SerializerSession _session;
        public PumpStateHistoryReadInfoResponse ReadInfoResponse { get; set; }
        public MultiPacketHandler CurrentMultiPacketHandler { get; set; }

        public List<MultiPacketHandler> MultiPacketHandlers { get; set; } = new List<MultiPacketHandler>();

        public PumpDataHistory(SerializerSession session)
        {
            _session = session;
        }

        public void AddMultiHandler(InitiateMultiPacketTransferResponse response)
        {
            MultiPacketHandler handler = new MultiPacketHandler(_session);
            handler.Init = response;
            MultiPacketHandlers.Add(handler);
            CurrentMultiPacketHandler = handler;
        }


        public void TestHistory()
        {

        }

        public override string ToString()
        {
            if (this.ReadInfoResponse!=null)
            {
                return $"{this.ReadInfoResponse.ToString()} ({this.CurrentMultiPacketHandler.PumpStateHistory.Count})";
            }
            return "(No dates)";
        }
    }
}
