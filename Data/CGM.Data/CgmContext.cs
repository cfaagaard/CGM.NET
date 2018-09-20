using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CGM.Data.Model;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Responses.Events;
using Newtonsoft.Json;
using System.Linq;
using CGM.Communication.Extensions;
using System.Threading.Tasks;

namespace CGM.Data
{


    public class CgmContext : DbContext
    {

        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<HistoryStatus> HistoryStatus { get; set; }
        public DbSet<DeviceStatus> DeviceStatus { get; set; }

        public CgmContext():base()
        {
            this.Database.Migrate();
            
        }


        public void UpdateConfiguration<T>(T configuration)
        {
            var config=this.Configuration.FirstOrDefault(e=>e.ConfigurationName== typeof(T).Name);
            config.ConfigurationValue = JsonConvert.SerializeObject(configuration);
            this.SaveChanges();

        }
        public void AddConfiguration<T>(T configuration)
        {
            Configuration config = new Configuration();
            config.ConfigurationName = typeof(T).Name;
            config.ConfigurationValue = JsonConvert.SerializeObject(configuration);
            this.Add(config);
            this.SaveChanges();
        }

        public T GetConfiguration<T>()
        {

            Configuration configuration = this.Configuration.FirstOrDefault(v => v.ConfigurationName == typeof(T).Name);
            if (configuration != null && !string.IsNullOrEmpty(configuration.ConfigurationValue))
            {
                return JsonConvert.DeserializeObject<T>(configuration.ConfigurationValue);
            }
            return default(T);
        }

        public Task<List<BasePumpEvent>> GetEventsAsync()
        {
           
            SerializerSession session = new SerializerSession();
            session.PumpSettings = this.GetConfiguration<PumpSettings>();
            Serializer serializer = new Serializer(session);

            return this.History.OrderByDescending(e => e.Rtc).Take(100).Select(e => serializer.Deserialize<BasePumpEvent>(e.HistoryBytes.GetBytes())).ToListAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=CgmData.db");
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>().HasKey(e => e.ConfigurationName);
            modelBuilder.Entity<Device>().HasKey(e => e.SerialNumber);
            modelBuilder.Entity<History>().HasKey(e => e.Key);
            modelBuilder.Entity<HistoryStatus>().HasKey(e => e.Key);
            modelBuilder.Entity<DeviceStatus>().HasKey(e => e.DeviceStatusKey);

            //var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<CgmContext>(modelBuilder);
            //Database.SetInitializer(sqliteConnectionInitializer);
        }
    }

}
