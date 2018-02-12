using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum HistoryDataTypeEnum:byte
    {
        //what is 0x01.....?????
        //UNKNOWN=0X01,
        Pump = 0x02,
        Sensor = 0x03
        //is there a 0x04 and 5.........?????
    }
}
