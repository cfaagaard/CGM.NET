using CGM.Communication.Extensions;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CGM.Communication.Common.Serialize
{
    [Serializable]
    public class SegementHandler
    {
        private ILogger Logger = ApplicationLogging.CreateLogger<SegementHandler>();
        private int block_size = 2048;

        public InitiateMultiPacketTransferResponse Init { get; set; }
        public PumpStateHistoryStart HistoryStart { get; set; }
        public List<BasePumpEvent> Events { get; set; } = new List<BasePumpEvent>();
        public List<Packet> Packets { get; set; } = new List<Packet>();
        public int ReadLength { get; set; } = 0;
        public bool Errors { get; set; } = false;
        private MultiPacketHandler _handler;
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public List<byte> Bytes { get; set; } = new List<byte>();
        public byte[] Decompressed { get; set; }
        public SegementHandler(MultiPacketHandler handler, InitiateMultiPacketTransferResponse response)
        {
            _handler = handler;
            Init = response;
        }

        public void AddHistory(Packet history)
        {
            Packets.Add(history);
        }

        public void GetHistoryEvents()
        {
            Bytes = new List<byte>();
            Events = new List<BasePumpEvent>();
            Segments = new List<Segment>();
            Decompressed = null;

            if (this.Packets.Count == 0)
            {
                Logger.LogError($"No history read: {this._handler.ReadInfoResponse.HistoryDataType.ToString()}");
                Errors = true;
                return;
            }

            if (this.Packets.Count != this.Init.PacketsToFetch)
            {
                Logger.LogError($"Wrong number of PacketsToFetch  {this._handler.ReadInfoResponse.HistoryDataType.ToString()} (expected {this.Init.PacketsToFetch}/{this.Packets.Count})");
                Errors = true;
                return;
            }

            //check length and packet number is ok

            //var numbers=this.Packets.Select(e => e.PacketNumber);

            //for (int i = 0; i < this.Packets.Count; i++)
            //{

            //}
            

            foreach (var item in this.Packets.OrderBy(e => e.PacketNumber))
            {
                List<byte> temp = new List<byte>();
                temp.AddRange(item.FullMessage.Reverse());

                if (item.PacketNumber != this.Init.PacketsToFetch - 1)
                {
                    if (temp.Count>= this.Init.PacketSize)
                    {
                        Bytes.AddRange(temp.GetRange(0, this.Init.PacketSize));
                    }
                    else
                    {
                        Logger.LogError($"Wrong length of bytes in packetnumber  {item.PacketNumber} (expected {temp.Count}/{ this.Init.PacketSize})");
                        Errors = true;
                        return;
                    }
                    
                }
                else
                {
                    if (temp.Count >= this.Init.LastPacketSize)
                    {
                        Bytes.AddRange(temp.GetRange(0, this.Init.LastPacketSize));
                    }
                    else
                    {
                        Logger.LogError($"Wrong length of bytes in packetnumber  {item.PacketNumber} (expected {temp.Count}/{ this.Init.PacketSize})");
                        Errors = true;
                        return;
                    }
                    
                }
            }




            if (this.Init.SegmentSize != Bytes.Count)
            {
                Logger.LogError($"Wrong segmentsize in {this._handler.ReadInfoResponse.HistoryDataType.ToString()} ({this.Init.SegmentSize}/{Bytes.Count})");
                Errors = true;
                return;
            }

            HistoryStart = _handler._seri.Deserialize<PumpStateHistoryStart>(Bytes.ToArray());

            if (HistoryStart.historySizeCompressed != HistoryStart.AllBytesNoHeader.Length)
            {
                Logger.LogError($"Wrong historySizeCompressed in {this._handler.ReadInfoResponse.HistoryDataType.ToString()} ({HistoryStart.historySizeCompressed}/{HistoryStart.AllBytesNoHeader.Length})");
                Errors = true;
                return;
            }



            byte[] blockpayload = new byte[HistoryStart.historySizeUncompressed];
            if (HistoryStart.historyCompressed == 0x01)
            {
                try
                {
                    using (Stream stream = new MemoryStream(HistoryStart.AllBytesNoHeader))
                    using (var decompressed1 = new LzoStream(stream, CompressionMode.Decompress, false, HistoryStart.historySizeUncompressed))
                    {
                        // blockpayload = decompressed1.ToArray(HistoryStart.historySizeUncompressed);
                        blockpayload = decompressed1.ToByteArray();
                        //for debugging...
                        Decompressed = blockpayload;
                    }
                    
                }
                catch (Exception ex)
                {

                    Logger.LogError(ex.Message);
                    return;
                }
            }
            else
            {
                blockpayload = HistoryStart.AllBytesNoHeader;
            }

            int remainder = (int)Math.IEEERemainder(blockpayload.Length, block_size);

            if ((blockpayload.Length % block_size) != 0)
            {
                Logger.LogError($"{this._handler.ReadInfoResponse.HistoryDataType.ToString()}: Block payload size is not a multiple of 2048 ({blockpayload.Length.ToString()} -> {remainder.ToString()})");
                Errors = true;
                return;
            }

            var length = blockpayload.Length / block_size;

            for (int i = 0; i < length; i++)
            {
                var blockData = blockpayload.ToList().GetRange(i * block_size, block_size);
                Segments.Add(new Segment(blockData.ToArray(), _handler));
            }

            this.Segments.Where(e => e.IsBlockDataCorrect).ToList().ForEach(e => e.GetEvents());
            this.Segments.ForEach(e => this.Events.AddRange(e.Events));
        }

        public List<int> GetMissingSegments()
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < this.Init.PacketsToFetch; i++)
            {

                var packet = this.Packets.FirstOrDefault(e => e.PacketNumber == i);
                if (packet == null)
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }
    }
}
