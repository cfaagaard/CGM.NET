using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data
{
    public class HistoryStatus
    {
        [SQLite.MaxLength(500)]
        public string Key { get; set; }

        public int HistoryStatusType { get; set; }

        public int Status { get; set; }

        [SQLite.MaxLength(10)]
        public string Comment { get; set; }

    }

    public enum HistoryStatusTypeEnum: int
    {
        NightScout=1
    }
}
