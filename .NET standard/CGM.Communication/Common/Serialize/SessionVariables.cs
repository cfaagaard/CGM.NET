using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    public class SessionVariables
    {

        private int SessionNumber= 0;
        private int SequenceNumber = 0;
        private int CryptedSequenceNumber = 0;
        public int GetNextSessionNumber()
        {
            this.SessionNumber += 1;
            return this.SessionNumber;
        }

        public int GetNextSequenceNumber()
        {
            this.SequenceNumber += 1;
            return this.SequenceNumber;
        }

        public int GetCryptedSequenceNumber()
        {
            this.CryptedSequenceNumber += 1;
            return this.CryptedSequenceNumber;
        }
    }
}
