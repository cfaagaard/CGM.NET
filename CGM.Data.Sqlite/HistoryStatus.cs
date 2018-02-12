using System;
using System.Collections.Generic;
using System.Text;

namespace  CMG.Data.Sqlite
{
    public class HistoryStatus
    {
        [SQLite.MaxLength(500)]
        public string Key { get; set; }

        public int Status { get; set; }

        [SQLite.MaxLength(10)]
        public string Comment { get; set; }

    }
}
