using System;
using System.Collections.Generic;

namespace CGM.Data.Model
{
   
    public partial class Device
    {
        public string SerialNumber { get; set; }

        public string SerialNumberFull { get; set; }

        public string Name { get; set; }
    
        public string LinkMac { get; set; }
     
        public string PumpMac { get; set; }
      
        public string RadioChannel { get; set; }
    
        public string LinkKey { get; set; }
 
        public override string ToString()
        {
            return $"{Name} - {SerialNumberFull}";
        }
    }
}
