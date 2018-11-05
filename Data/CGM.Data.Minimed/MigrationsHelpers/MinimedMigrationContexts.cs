using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Data.Minimed
{

    //for design time use only
    class SqliteDbContext : MinimedContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=minimed.db");
    }

    class SqlDbContext : MinimedContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("");
    }
}
