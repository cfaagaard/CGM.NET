using CGM.Communication.Extensions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace  CMG.Data.Sqlite
{
    public class SqliteSetting
    {
        [SQLite.PrimaryKey]
        public int SettingId { get; set; } = 1;

        [SQLite.MaxLength(10000)]
        public string SettingJson { get; set; }
        
    }
}
