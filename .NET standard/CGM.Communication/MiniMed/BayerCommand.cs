using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System.Text;


namespace CGM.Communication.MiniMed
{
    [BinaryType]
    public class BayerCommand : AstmStart
    {




        public BayerCommand()
        {

        }

        public BayerCommand(byte[] command)
        {
            this.ReportId = 0x00;
            this.Direction = new byte[] { 0x00, 0x00, 0x00 };
            this.Command = command;
            this.CommandLength = 0x00;
        }

        public BayerCommand(string command) : this(Encoding.ASCII.GetBytes(command))
        {

        }

        public BayerCommand(AstmAscii command) : this(new byte[] { (byte)command })
        {

        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), Encoding.ASCII.GetString(Command));
        }
    }

}
