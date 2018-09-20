using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    [Serializable]
    public class Segment
    {
     
        private MultiPacketHandler _handler;
        public List<byte> Block { get; set; }= new List<byte>();

        public ushort Datasize { get; set; }
        public List<byte> BlockData { get; set; }
        public ushort BlockCheckSum { get; set; }

        public int CalculateCheckSum { get; set; }

        public List<BasePumpEvent> Events { get; set; } = new List<BasePumpEvent>();

        public bool IsBlockDataCorrect { get; set; }

        public Segment(byte[] block, MultiPacketHandler handler)
        {
            _handler = handler;

            Block.AddRange(block);

            Datasize = block.GetUInt16BigE(block.Length - 4);
            if (Datasize > 0 && Datasize <= block.Length)
            {
                BlockData = Block.GetRange(0, Datasize);
                BlockCheckSum = Block.ToArray().GetUInt16BigE(block.Length - 2);
                //maybe not the 0xffff.... test
                CalculateCheckSum =BlockData.ToArray().GetCrc16citt() & 0xffff;
                this.IsBlockDataCorrect = BlockCheckSum == CalculateCheckSum;
            }

        }
        public void GetEvents()
        {
            if (this.IsBlockDataCorrect)
            {
                GetEvents(BlockData, 0);
            }
            else
            {
                throw new Exception("IsBlockDataCorrect is false");
            }
           
        }
        public void GetEvents(List<byte> bytes, int start)
        {
            var length = bytes[start + 2];
            if ((length + start)>bytes.Count || length==0)
            {
                return;
            }

            var bytesMessage = bytes.GetRange(start, length).ToArray();
           
            try
            {
                var eventmessage = _handler._seri.Deserialize<BasePumpEvent>(bytesMessage);
                eventmessage.Index = start;
                eventmessage.HistoryDataType = _handler.ReadInfoResponse.HistoryDataTypeRaw;
                Events.Add(eventmessage);

                int newstart = start + length;
                if (newstart < bytes.Count)
                {
                    GetEvents(bytes, newstart);
                }
            }
            catch (Exception)
            {

               // throw;
            }
            

        }

        public override string ToString()
        {
            return $"events {this.Events.Count}";
        }
    }
}
