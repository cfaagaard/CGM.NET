using CGM.Communication.Extensions;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.Extensions;
using System.IO;

namespace CGM.Communication.Common.Serialize
{
    public class SegementHandler
    {
        private ILogger Logger = ApplicationLogging.CreateLogger<SegementHandler>();
        private int block_size = 2048;

        public InitiateMultiPacketTransferResponse Init { get; set; }
        public PumpStateHistoryStart HistoryStart { get; set; }
        public List<PumpEvent> Events { get; set; } = new List<PumpEvent>();
        public List<PumpStateHistory> PumpStateHistory { get; set; } = new List<PumpStateHistory>();
        public int ReadLength { get; set; } = 0;
        public bool Errors { get; set; } = false;
        private MultiPacketHandler _handler;


        public SegementHandler(MultiPacketHandler handler, InitiateMultiPacketTransferResponse response)
        {
            _handler = handler;
            Init = response;
        }

        public void AddHistory(PumpStateHistory history)
        {
            PumpStateHistory.Add(history);
        }

        public void GetHistoryEvents()
        {

            Events = new List<PumpEvent>();
            //List<byte> segmentbytes = new List<byte>();

            if (this.PumpStateHistory.Count == 0)
            {
                Logger.LogError($"No history read: {this._handler.ReadInfoResponse.HistoryDataType.ToString()}");
                Errors = true;
                return;
            }

            if (this.PumpStateHistory.Count != this.Init.PacketsToFetch)
            {
                Logger.LogError($"Wrong number of PacketsToFetch  {this._handler.ReadInfoResponse.HistoryDataType.ToString()} (expected {this.Init.PacketsToFetch}/{this.PumpStateHistory.Count})");
                Errors = true;
                return;
            }


            //var all = PumpStateHistory.OrderBy(e => e.PacketNumber).Select(e => e.Message).ToList();
            List<byte> bytes = new List<byte>();

            foreach (var item in this.PumpStateHistory.OrderBy(e => e.PacketNumber))
            {
                List<byte> temp = new List<byte>();
                temp.AddRange(item.FullMessage.Reverse());
                if (item.PacketNumber!=this.Init.PacketsToFetch-1)
                {
                    bytes.AddRange(temp.GetRange(0, this.Init.PacketSize));
                }
                else
                {
                    bytes.AddRange(temp.GetRange(0, this.Init.LastPacketSize));
                }
            }

            //for (int i = 0; i < this.Init.PacketsToFetch; i++)
            //{
            //    List<byte> temp = new List<byte>();
            //    temp.AddRange(this.PumpStateHistory[i].FullMessage.Reverse());
            //    if (i<(this.Init.PacketsToFetch-1))
            //    {
            //        bytes.AddRange(temp.GetRange(0, this.Init.PacketSize));
            //    }
            //    else
            //    {
            //        bytes.AddRange(temp.GetRange(0, this.Init.LastPacketSize));
            //    }
              
            //}

            //var all = PumpStateHistory.OrderBy(e => e.PacketNumber).Select(e => e.FullMessage).ToList();
            //all.ForEach(e => segmentbytes.AddRange(e));


            if (this.Init.SegmentSize != bytes.Count)
            {
                Logger.LogError($"Wrong segmentsize in {this._handler.ReadInfoResponse.HistoryDataType.ToString()} ({this.Init.SegmentSize}/{bytes.Count})");
                Errors = true;
                return;
            }

            HistoryStart = _handler._seri.Deserialize<PumpStateHistoryStart>(bytes.ToArray());


            byte[] blockpayload = new byte[HistoryStart.historySizeUncompressed];
            //byte[] blockpayload2 = new byte[HistoryStart.historySizeUncompressed];
            //byte[] blockpayload3 = new byte[HistoryStart.historySizeUncompressed];
            if (HistoryStart.historyCompressed == 0x01)
            {
                try
                {
                    using (Stream stream = new MemoryStream(HistoryStart.AllBytesNoHeader))
                    //using (Stream stream = new MemoryStream(segmentbytes.ToArray()))
                    using (var decompressed1 = new LzoStream(stream, CompressionMode.Decompress, false, HistoryStart.historySizeUncompressed))
                    //using (var decompressed1 = new LzoStream(stream, CompressionMode.Decompress, false ))
                    {
                        //blockpayload = decompressed1.ToByteArray(HistoryStart.historySizeUncompressed);


                        //using (MemoryStream stream3 = new MemoryStream(HistoryStart.historySizeUncompressed))
                        //{
                        //    decompressed1.CopyTo(stream3);
                        //    blockpayload = stream3.ToArray();
                        //}

                        blockpayload = decompressed1.ToArray(HistoryStart.historySizeUncompressed);

                        //blockpayload = decompressed1.ToByteArray();
                    }

                    //using (Stream stream2 = new MemoryStream(HistoryStart.AllBytesNoHeader))
                    //using (var decompressed1 = new LzoStream(stream2, CompressionMode.Decompress, false, HistoryStart.historySizeUncompressed))
                    //{

                    //    blockpayload2 = decompressed1.ToArray(HistoryStart.historySizeUncompressed);



                    //}

                    //using (Stream stream4 = new MemoryStream(HistoryStart.AllBytesNoHeader))
                    //using (var decompressed1 = new LzoStream(stream4, CompressionMode.Decompress, false, HistoryStart.historySizeUncompressed))
                    //{

                    //    //blockpayload2 = decompressed1.ToArray(HistoryStart.historySizeUncompressed);
                    //    using (MemoryStream stream3 = new MemoryStream())
                    //    {
                    //        decompressed1.CopyTo(stream3);
                    //        blockpayload3 = stream3.ToArray();
                    //    }


                    //}
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
                var blockStart = i * block_size;
                var blockSize = blockpayload.GetUInt16BigE(((i + 1) * block_size) - 4);
                var blockChecksum = blockpayload.GetUInt16BigE(((i + 1) * block_size) - 2);
                if ((blockStart + blockSize) <= blockpayload.Length)
                {
                    var blockData = blockpayload.ToList().GetRange(blockStart, blockSize);
                    var calculatedChecksum2 = Crc16Ccitt.CRC16CCITT(blockData.ToArray(), 0xFFFF, 0x1021, blockSize);

                    if (blockChecksum == calculatedChecksum2)
                    {
                        GetEvents(blockData, 0);
                    }
                    else
                    {
                        Logger.LogError($"{this._handler.ReadInfoResponse.HistoryDataType.ToString()}: CRC16CCITT does not match.");
                        Errors = true;
                        return;
                    }
                }
            }
        }

        private void GetEvents(List<byte> bytes, int start)
        {
            var length = bytes[start + 2];
            var bytesMessage = bytes.GetRange(start, length).ToArray();
            var eventmessage = _handler._seri.Deserialize<PumpEvent>(bytesMessage);

            Events.Add(eventmessage);

            int newstart = start + length;
            if (newstart < bytes.Count)
            {
                GetEvents(bytes, newstart);
            }

        }


        public List<int> GetMissingSegments()
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < this.Init.PacketsToFetch; i++)
            {
                if (this.PumpStateHistory.Count>=i)
                {
                    if (this.PumpStateHistory[i].PacketNumber != i)
                    {
                        indexes.Add(i);
                        i += 1;
                    }
                }
                else
                {
                    indexes.Add(i);
                    i += 1;
                }
               
            }
            return indexes;
        }
    }
}
