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
        public List<PumpStateHistory> PumpStateHistory { get; set; } = new List<PumpStateHistory>();
        public InitiateMultiPacketTransferResponse Init { get; set; }
        public List<PumpEvent> Events { get; set; } = new List<PumpEvent>();
        public int ReadLength { get; set; } = 0;
        public PumpStateHistoryReadInfoResponse ReadInfoResponse { get; set; }

        public PumpStateHistoryStart HistoryStart { get; set; }

        public int ExpectedMessages
        {
            get
            {
                if (this.Init != null)
                {
                    var expectedMessages = (this.Init.SegmentSize - this.Init.LastPacketSize) / this.Init.PacketSize;
                    //and the last message.
                    expectedMessages += 1;
                    return expectedMessages;
                }
                return 0;
            }
        }

        Serializer _seri;
        public MultiPacketHandler(SerializerSession session)
        {
            _session = session;
            _seri = new Serializer(_session);
        }

        public void AddHistory(PumpStateHistory history)
        {
            PumpStateHistory.Add(history);
            ReadLength += history.Message.Length;
        }

        public void GetHistoryEvents()
        {
            int block_size = 2048;
            byte[] blockpayload;
            Events = new List<PumpEvent>();
            List<byte> segmentbytes = new List<byte>();

            var all = PumpStateHistory.Select(e => e.Message);

            foreach (var item in all)
            {
                segmentbytes.AddRange(item);
            }

            HistoryStart = _seri.Deserialize<PumpStateHistoryStart>(segmentbytes.ToArray());
            if (this.Init.SegmentSize != this.ReadLength)
            {
                Logger.LogError($"Wrong segmentsize in {this.ReadInfoResponse.HistoryDataType.ToString()} ({this.Init.SegmentSize}/{this.ReadLength})");
                return;
            }

            if (HistoryStart.historyCompressed == 0x01)
            {
                using (Stream stream = new MemoryStream(HistoryStart.AllBytesNoHeader))
                using (var ms = new MemoryStream())
                using (var decompressed1 = new LzoStream(stream, CompressionMode.Decompress, false, HistoryStart.historySizeUncompressed))
                {
                    decompressed1.CopyTo(ms);
                    blockpayload = ms.ToArray();
                }
            }
            else
            {
                blockpayload = HistoryStart.AllBytesNoHeader;
            }

            if ((blockpayload.Length % block_size) != 0)
            {
                double remainder = Math.IEEERemainder(blockpayload.Length,block_size);
                Logger.LogError($"Block payload size is not a multiple of 2048 ({blockpayload.Length.ToString()} -> {remainder.ToString()})");
                return;

            }

            //var length = Math.Ceiling((double)blockpayload.Length / block_size);// blockpayload.Length / block_size;
            var length = blockpayload.Length / block_size;
            for (int i = 0; i < length; i++)
            {
                var blockStart = i * block_size;
                var blockSize = blockpayload.GetUInt16BigE(((i + 1) * block_size) - 4);
                var blockChecksum = blockpayload.GetUInt16BigE(((i + 1) * block_size) - 2);

                //if (blockSize <= block_size)
                //{
                var blockData = blockpayload.ToList().GetRange(blockStart, blockSize);
                var calculatedChecksum2 = Crc16Ccitt.CRC16CCITT(blockData.ToArray(), 0xFFFF, 0x1021, blockSize);

                if (blockChecksum != calculatedChecksum2)
                {

                }
                else
                {
                    GetEvents(blockData, 0);
                }
                //}
                //else
                //{

                //}
            }
        }

        private void GetEvents(List<byte> bytes, int start)
        {
            var length = bytes[start + 2];
            var bytesMessage = bytes.GetRange(start, length).ToArray();
            var eventmessage = _seri.Deserialize<PumpEvent>(bytesMessage);

            Events.Add(eventmessage);

            int newstart = start + length;
            if (newstart < bytes.Count)
            {
                GetEvents(bytes, newstart);
            }

        }
    }
}
