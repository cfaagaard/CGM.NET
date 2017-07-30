using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CGM.Communication.Common.Serialize
{
    public class MultiPacketHandler
    {
        private SerializerSession _session;
        public List<PumpStateHistory> PumpStateHistory { get; set; } = new List<PumpStateHistory>();
        public InitiateMultiPacketTransferResponse Init { get; set; }

        public MultiPacketHandler(SerializerSession session)
        {
            _session = session;
        }

        public PumpStateHistoryStart ReadAllhistory()
        {
            var all = PumpStateHistory.Select(e => e.Message);

            List<byte> segmentbytes = new List<byte>();

            foreach (var item in all)
            {
                segmentbytes.AddRange(item);
            }
            Serializer seri = new Serializer(_session);
           var message= seri.Deserialize<PumpStateHistoryStart>(segmentbytes.ToArray());
            return message;
        }
    }
}
