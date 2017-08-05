using CGM.Communication.MiniMed.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using CGM.Communication.MiniMed.Responses.Events;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.Common.Serialize
{
    public class MultiPacketHandler
    {
        private SerializerSession _session;
        public List<PumpStateHistory> PumpStateHistory { get; set; } = new List<PumpStateHistory>();
        public InitiateMultiPacketTransferResponse Init { get; set; }
        public List<PumpEvent> Events { get; set; } = new List<PumpEvent>();
        public int ReadLength { get; set; } = 0;
        public PumpStateHistoryReadInfoResponse ReadInfoResponse { get; set; }

        public int ExpectedMessages { get {
                if (this.Init!=null )
                {
                    return (this.Init.SegmentSize - this.Init.LastPacketSize) / this.Init.PacketSize;
                }
                return 0;
            } }

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

            Events = new List<PumpEvent>();
            var all = PumpStateHistory.Select(e => e.Message);

            List<byte> segmentbytes = new List<byte>();

            foreach (var item in all)
            {
                segmentbytes.AddRange(item);
            }
            int block_size = 2048;

     
            var message = _seri.Deserialize<PumpStateHistoryStart>(segmentbytes.ToArray());
            //32.768
            //32.825

            //var test = CLZF2.Decompress(message.AllBytesNoHeader);

            using (Stream stream = new MemoryStream(message.AllBytesNoHeader))
            using (var ms = new MemoryStream())
            using (var decompressed1 = new LzoStream(stream, CompressionMode.Decompress))
            {
                decompressed1.CopyTo(ms);
                byte[] decompressed = ms.ToArray();

                List<List<byte>> blocks2 = new List<List<byte>>();
                var length = decompressed.Length / block_size;
                for (int i = 0; i < length; i++)
                {
                    var blockStart = i * block_size;
                    var blockSize =  decompressed.GetUInt16BigE(((i + 1) * block_size) - 4); //2039;//
                    var blockChecksum = decompressed.GetUInt16BigE(((i + 1) * block_size) - 2);
                    if (blockSize <= block_size)
                    {
                        var blockData = decompressed.ToList().GetRange(blockStart, blockSize);

                        //var calculatedChecksum = Crc16Ccitt.CRC16CCITT(blockData.ToArray(), 0xFFFF, 0x0000, blockSize);
                        var calculatedChecksum2 = Crc16Ccitt.CRC16CCITT(blockData.ToArray(), 0xFFFF, 0x1021, blockSize);

                        if (blockChecksum != calculatedChecksum2)
                        {

                        }
                        else
                        {
                            GetEvents(blockData, 0);
                            blocks2.Add(blockData);
                        }
                    }
                    else
                    {

                    }
                }

                //foreach (var item in blocks2)
                //{
                //    GetEvents(item, 0);
                //}

            }
        }

        private void GetEvents(List<byte> bytes, int start)
        {
            var length = bytes[start + 2];

            var bytesMessage = bytes.GetRange(start, length).ToArray();

            var eventmessage = _seri.Deserialize<PumpEvent>(bytesMessage);

            //PumpEvent ev = new PumpEvent();
            //ev.EventType = (EventTypeEnum)bytes[start];
            //ev.Source = bytes[start + 1];
            //ev.Length = bytes[start + 2];
            //ev.Rtc = bytes.ToArray().GetInt32BigE(start + 3);
            //ev.Offset = bytes.ToArray().GetInt32BigE(start + 7);
            //ev.Message = bytes.GetRange(start + 8, ev.Length - (8)).ToArray();
            Events.Add(eventmessage);
            int newstart = start + length;
            if (newstart < bytes.Count)
            {
                GetEvents(bytes, newstart);
            }

        }
    }
}
