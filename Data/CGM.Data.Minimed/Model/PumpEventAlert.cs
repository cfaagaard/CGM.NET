using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed.Model
{
    public class PumpEventAlert
    {
        public int PumpEventAlertId { get; set; }
        public string PumpEventAlertName { get; set; }

        public string PumpEventAlertFullName { get; set; }

        public ICollection<PumpEvent> PumpEvents { get; set; }

        public PumpEventAlert()
        {
            PumpEvents = new HashSet<PumpEvent>();
        }
    }
}
