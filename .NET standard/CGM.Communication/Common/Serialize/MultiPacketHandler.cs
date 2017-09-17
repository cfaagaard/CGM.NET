using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using CGM.Communication.MiniMed.Responses.Events;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;

namespace CGM.Communication.Common.Serialize
{
    public class MultiPacketHandler
    {
        private ILogger Logger = ApplicationLogging.CreateLogger<MultiPacketHandler>();
        private SerializerSession _session;
        internal Serializer _seri;

        public List<SegementHandler> Segments { get; set; } = new List<SegementHandler>();
        public PumpStateHistoryReadInfoResponse ReadInfoResponse { get; set; }
        public bool WaitingForSegment { get; set; } = false;

        public SegementHandler CurrentSegment { get; set; }

        public MultiPacketHandler(SerializerSession session)
        {
            _session = session;
            _seri = new Serializer(_session);
        }

        public void AddSegmentHandler(InitiateMultiPacketTransferResponse response)
        {
            this.CurrentSegment = new SegementHandler(this, response);
            this.Segments.Add(CurrentSegment);
            WaitingForSegment = true;
        }

        public void GetHistoryEvents()
        {
            if (Segments!=null && Segments.Count>0)
            {
                Segments.ForEach(e => e.GetHistoryEvents());
            }
           
        }

        public List<PumpEvent> JoinAllEvents()
        {
            List<PumpEvent> all = new List<PumpEvent>();
            Segments.ForEach(f => all.AddRange(f.Events));
            return all.OrderBy(e => e.Timestamp).ToList(); ;
        }
    }
}
