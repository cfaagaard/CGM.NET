using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Infrastructur;
using System;
using System.Collections.Generic;
using CGM.Communication.Extensions;
using System.Linq;

namespace CGM.Communication.MiniMed
{
    [BinaryType]
    public class MedtronicMessage3 : IBinaryType, IBinaryDeserializationSetting
    {

        [BinaryElement(0)]
        public byte CommandAction { get; set; }
        [BinaryElement(1)]
        [BinaryElementLogicLength(Add = -2)]
        public byte MessageLength { get; set; }

        public AstmCommandAction CommandActionName { get { return (AstmCommandAction)CommandAction; } }

        //[BinaryElement(0, Direction = DirectionEnum.Reverse, Length = 2)]
        //[BinaryElementLogicCrc16Ciit]
        //public byte[] Crc { get; set; }

        public bool IsCrcCorrect { get; set; }

        public MedtronicMessage3()
        {

        }

        public MedtronicMessage3(byte commandAction)
        {
            this.CommandAction = commandAction;
            this.MessageLength = 0x00;
            //this.Crc = new byte[] { 0x00, 0x00 };
        }
        public MedtronicMessage3(AstmCommandAction commandAction) : this((byte)commandAction)
        {
        }


        public override string ToString()
        {
            return this.CommandActionName.ToString(); /*+ " - " +  this.GetType().Name.ToString() */
        }

        public void OnDeserialization(byte[] bytes, SerializerSession settings)
        {
            List<byte> list = new List<byte>();
            list.AddRange(bytes);
            var data = list.GetRange(0, list.Count - 2);
            var Crc = list.GetRange(list.Count - 2, 2).ToArray();
            var crc = data.ToArray().GetCrc16citt() & 0xffff;
            var ch = Crc.GetInt16(0) & 0xffff;
            this.IsCrcCorrect = crc == ch;
            if (IsCrcCorrect)
            {

            }
            else
            {

            }
        }
    }

}
