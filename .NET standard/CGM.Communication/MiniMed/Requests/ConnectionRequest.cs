using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed.Requests
{
    [BinaryType]
    public class ConnectionRequest: IBinaryType 
    {
        [BinaryElement(0)]
        public byte[] HMACbyte { get; set; }

        public ConnectionRequest()
        {

        }
        public ConnectionRequest(byte[] hmac) 
        {
            this.HMACbyte = hmac;
        }

        public override string ToString()
        {
            return this.GetType().Name.ToString();
        }
    }

}
