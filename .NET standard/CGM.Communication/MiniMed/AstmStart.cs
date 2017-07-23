using System;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Patterns;
using System.Collections.Generic;
using CGM.Communication.Extensions;
using System.Linq;

namespace CGM.Communication.MiniMed
{
    [BinaryType]
    public class AstmStart : IBinaryType, IBinarySerializationSetting
    {
        [BinaryElement(0)]
        public byte ReportId { get; set; }

        [BinaryElement(1)]
        public byte[] Direction { get; set; }
        public AstmDirection DirectionName
        {
            get
            {
                if (Direction[1] != 0x00)
                {
                    return AstmDirection.In;
                }
                else
                {
                    return AstmDirection.Out;
                }
            }
        }

        [BinaryElement(4)]
        [BinaryElementLogicLength(From = 5)]
        public byte CommandLength { get; set; }

        [BinaryElement(5)]
        public byte[] Command { get; set; }

        [BinaryElement(7)]
        [MessageType(typeof(BayerStickInfoResponse), typeof(StickPattern))]
        [MessageType(typeof(MedtronicMessage2), typeof(MedtronicMessagePattern))]
        public object Message2 { get; set; }

        public AstmStart()
        {

        }

        public AstmStart(byte[] command)
        {
            this.ReportId = 0x00;
            this.Direction = new byte[] { 0x00, 0x00, 0x00 };
            this.Command = command;
            this.CommandLength = (byte)command.Length;
        }

        public AstmStart(string command) : this(System.Text.Encoding.ASCII.GetBytes(command))
        {

        }

        public AstmStart(AstmAscii command) : this(new byte[] { (byte)command })
        {

        }



        public override string ToString()
        {
            string commandName = "";
            var signes = Command.Where(e => e >= 0x30);
            
            if (signes.Count()>0)
            {
                commandName = System.Text.Encoding.ASCII.GetString(signes.ToArray());
            }
            var signes2 = Command.Where(e => Enum.IsDefined(typeof(AstmAscii),e)).Select(e=> (AstmAscii)e);

            if (signes2.Count()>0)
            {
                commandName = signes2.First().ToString();
            }


            string returnStr = string.Format("{0} ({1})", DirectionName.ToString(), commandName);

            if (Message2 != null)
            {
                returnStr += Message2.ToString();
            }
            return returnStr;
        }

        public void OnSerialization(List<byte> bytes, SerializerSession settings)
        {
            if (bytes.Count > 32)
            {
                //medtronic length
                bytes[33] = (byte)(bytes.Count - 38);
            }

            if (bytes.Count > 37)
            {
                if (bytes[37] == 0x00)
                {
                    var temp = bytes.GetRange(5, bytes.Count - 5).ToArray();
                    bytes[37] = temp.OneByteSum();
                }

            }

        }
    }

}
