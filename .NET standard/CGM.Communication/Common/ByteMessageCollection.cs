using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using CGM.Communication.Common.Serialize;

namespace CGM.Communication.Common
{
    public class ByteMessageCollection
    {
        private SerializerSession _settings;
        public List<ByteMessage> Messages { get; set; } = new List<ByteMessage>();

        private ByteMessage _message;

        private int _currentNumber = 1;

        public ByteMessageCollection(SerializerSession settings)
        {
            _settings = settings;
            _message = new ByteMessage(_settings);
        }

        public void Add(string bytesAsString)
        {
            Add(bytesAsString.GetBytes(), _currentNumber, "");
            _currentNumber += 1;
        }

        public void Add(string bytesAsString, int number, string comment)
        {
            Add(bytesAsString.GetBytes(), number, comment);
        }
        public void Add(byte[] bytes, int number, string comment)
        {
            
            ByteFrame bf = new ByteFrame() { Framenumber = number, Bytes = bytes, Comment=comment };

            if (_message.CanAdd(bf))
            {
                _message.AddFrame(bf);
            }

            if (_message.EndOfMessage)
            {
                Messages.Add(_message);
                _message = new ByteMessage(_settings);
               
            }
        }


        public void AnalyzeMessages()
        {
            Messages.ForEach(e => e.Analyze());
        }


    }
}
