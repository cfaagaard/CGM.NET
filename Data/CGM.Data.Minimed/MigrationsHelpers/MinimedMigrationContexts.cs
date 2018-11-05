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
            => options.UseSqlServer("Server=tcp:csrb7vo2oj.database.windows.net,1433;Initial Catalog=CGMNET;Persist Security Info=False;User ID=DbAdmin;Password=XY4711xy;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }
}
