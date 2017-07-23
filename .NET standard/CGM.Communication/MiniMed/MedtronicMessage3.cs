using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.MiniMed
{
    [BinaryType]
    public class MedtronicMessage3 : IBinaryType
    {

        [BinaryElement(0)]
        public byte CommandAction { get; set; }
        [BinaryElement(1)]
        [BinaryElementLogicLength(Add =-2)]
        public byte MessageLength { get; set; }

        public AstmCommandAction CommandActionName { get { return (AstmCommandAction)CommandAction; } }





        public MedtronicMessage3()
        {

        }

        public MedtronicMessage3(byte commandAction) 
        {
            this.CommandAction = commandAction;
            this.MessageLength = 0x00;
        }
        public MedtronicMessage3(AstmCommandAction commandAction) : this((byte)commandAction)
        {
        }


        public override string ToString()
        {
            return this.CommandActionName.ToString(); /*+ " - " +  this.GetType().Name.ToString() */  
        }
    }

}
