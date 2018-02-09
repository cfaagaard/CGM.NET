using System;
using System.Collections.Generic;

namespace CGM.Communication.Data
{
   
    public partial class Device
    {
        [SQLite.PrimaryKey,SQLite.MaxLength(50)]
        public string SerialNumber { get; set; }

        [SQLite.MaxLength(50)]
        public string SerialNumberFull { get; set; }

        [SQLite.MaxLength(50)]
        public string Name { get; set; }
        [SQLite.MaxLength(250)]
        public string LinkMac { get; set; }
        [SQLite.MaxLength(250)]
        public string PumpMac { get; set; }
        [SQLite.MaxLength(50)]
        public string RadioChannel { get; set; }
        [SQLite.MaxLength(500)]
        public string LinkKey { get; set; }
        //[SQLite.MaxLength(3000)]
        //public string FullInfo { get; set; }

        public override string ToString()
        {
            return $"{Name} - {SerialNumberFull}";
        }
    }
}
