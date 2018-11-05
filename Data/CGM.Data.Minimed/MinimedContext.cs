using CGM.Data.Minimed.Model;
using CGM.Data.Minimed.Model.Owned;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CGM.Data.Minimed
{
    public class MinimedContext : DbContext
    {
        public DbSet<Pump> Pumps { get; set; }
        public DbSet<PumpStatus> PumpStatus { get; set; }
        public DbSet<BayerStick> BayerSticks { get; set; }

        public DbSet<PumpEvent> PumpEvents { get; set; }

        public DbSet<SensorEvent> SensorEvents { get; set; }

        public DbSet<SensorReading> SensorReadings { get; set; }

        public DbSet<DailyTotal> DailyTotals { get; set; }

        public DbSet<Calibration> Calibrations { get; set; }

        public DbSet<EventType> EventTypes { get; set; }

        public DbSet<DataLoggerReading> DataLoggerReadings { get; set; }
        public DbSet<DataLogger> DataLoggers { get; set; }

        public DbSet<PumpEventAlert> PumpEventAlerts { get; set; }
        public DbSet<SensorReadingAlert> SensorReadingAlerts { get; set; }
        public MinimedContext() : base()
        {

        }
        public MinimedContext(DbContextOptions<MinimedContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=minimed.db");
            }

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DataLogger>().HasKey(e => e.DataLoggerId);
            modelBuilder.Entity<DataLogger>().Property(c => c.DataLoggerId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<DataLogger>().ToTable("DataLogger");



            modelBuilder.Entity<DataLoggerReading>().HasKey(e => e.DataLoggerReadingId);
            modelBuilder.Entity<DataLoggerReading>().Property(c => c.DataLoggerReadingId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<DataLoggerReading>().ToTable("DataLoggerReading");
            modelBuilder.Entity<DataLoggerReading>()
       .HasOne<DataLogger>(ps => ps.DataLogger)
       .WithMany(p => p.DataLoggerReadings)
       .HasForeignKey(ps => ps.DataLoggerId);

            modelBuilder.Entity<DataLoggerReading>()
.HasOne<BayerStick>(ps => ps.BayerStick)
.WithMany(p => p.DataLoggerReadings)
.HasForeignKey(ps => ps.BayerStickId);

            modelBuilder.Entity<Pump>().HasKey(e => e.PumpId);
            modelBuilder.Entity<Pump>().Property(c => c.PumpId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<Pump>().ToTable("Pump");

            modelBuilder.Entity<BayerStick>().HasKey(e => e.BayerStickId);
            modelBuilder.Entity<BayerStick>().Property(c => c.BayerStickId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<BayerStick>().ToTable("BayerStick");

            modelBuilder.Entity<PumpStatus>().HasKey(e => e.PumpStatusId);
            modelBuilder.Entity<PumpStatus>().Property(c => c.PumpStatusId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<PumpStatus>()
                .HasOne<Pump>(ps => ps.Pump)
                .WithMany(p => p.PumpStatus)
                .HasForeignKey(ps => ps.PumpId);
            modelBuilder.Entity<PumpStatus>().ToTable("PumpStatus");
            modelBuilder.Entity<PumpStatus>().OwnsOne(o => o.SgvDateTime);
            modelBuilder.Entity<PumpStatus>().OwnsOne(o => o.AlertDateTime);


            modelBuilder.Entity<SensorEvent>().HasKey(e => e.SensorEventId);
            modelBuilder.Entity<SensorEvent>().Property(c => c.SensorEventId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<SensorEvent>().ToTable("SensorEvent");
            modelBuilder.Entity<SensorEvent>()
               .HasOne<Pump>(ps => ps.Pump)
               .WithMany(p => p.SensorEvents)
               .HasForeignKey(ps => ps.PumpId);
            modelBuilder.Entity<SensorEvent>()
.HasOne<EventType>(ps => ps.EventType)
.WithMany(p => p.SensorEvents)
.HasForeignKey(ps => ps.EventTypeId);
            modelBuilder.Entity<SensorEvent>().OwnsOne(o => o.EventDate);



            modelBuilder.Entity<PumpEvent>().HasKey(e => e.PumpEventId);
            modelBuilder.Entity<PumpEvent>().Property(c => c.PumpEventId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<PumpEvent>().ToTable("PumpEvent");
            modelBuilder.Entity<PumpEvent>()
               .HasOne<Pump>(ps => ps.Pump)
               .WithMany(p => p.PumpEvents)
               .HasForeignKey(ps => ps.PumpId);

            modelBuilder.Entity<PumpEvent>()
.HasOne<EventType>(ps => ps.EventType)
.WithMany(p => p.PumpEvents)
.HasForeignKey(ps => ps.EventTypeId);

            modelBuilder.Entity<PumpEvent>()
.HasOne<PumpEventAlert>(ps => ps.PumpEventAlert)
.WithMany(p => p.PumpEvents)
.HasForeignKey(ps => ps.PumpEventAlertId);
            

            modelBuilder.Entity<PumpEvent>()
             .HasOne<DataLoggerReading>(ps => ps.DataLoggerReading)
             .WithMany(p => p.PumpEvents)
             .HasForeignKey(ps => ps.DataLoggerReadingId);
            modelBuilder.Entity<PumpEvent>().OwnsOne(o => o.EventDate);


            modelBuilder.Entity<SensorReading>().HasKey(e => e.SensorReadingId);
            modelBuilder.Entity<SensorReading>().Property(c => c.SensorReadingId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<SensorReading>().ToTable("SensorReading");
            modelBuilder.Entity<SensorReading>()
                .HasOne<SensorEvent>(ps => ps.SensorEvent)
                .WithMany(p => p.SensorReadings)
                .HasForeignKey(ps => ps.SensorEventId);
            modelBuilder.Entity<SensorReading>()
.HasOne<SensorReadingAlert>(ps => ps.SensorReadingAlert)
.WithMany(p => p.SensorReadings)
.HasForeignKey(ps => ps.SensorReadingAlertId);

            modelBuilder.Entity<DailyTotal>().HasKey(e => e.DailyTotalId);
            modelBuilder.Entity<DailyTotal>().Property(c => c.DailyTotalId).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<DailyTotal>().ToTable("DailyTotal");
            modelBuilder.Entity<DailyTotal>()
                .HasOne<PumpEvent>(ps => ps.PumpEvent)
                .WithMany(p => p.DailyTotals)
                .HasForeignKey(ps => ps.PumpEventId);

            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.METER_BG_AVERAGE);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.LOW_METER_BG);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.HIGH_METER_BG);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.MANUALLY_ENTERED_BG_AVERAGE);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.LOW_MANUALLY_ENTERED_BG);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.HIGH_MANUALLY_ENTERED_BG);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.BG_AVERAGE);

            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.TOTAL_INSULIN);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.BASAL_INSULIN);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.BOLUS_INSULIN);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.SG_AVERAGE);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.SG_STDDEV);

            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION);
            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.TOTAL_MANUAL_BOLUS_INSULIN);

            modelBuilder.Entity<DailyTotal>().OwnsOne(o => o.Date);


            modelBuilder.Entity<Calibration>().HasKey(e => e.CalibrationID);
            modelBuilder.Entity<Calibration>().Property(c => c.CalibrationID).ValueGeneratedOnAdd().UseSqlServerIdentityColumn();
            modelBuilder.Entity<Calibration>().ToTable("Calibration");
            modelBuilder.Entity<Calibration>()
                .HasOne<PumpEvent>(ps => ps.PumpEvent)
                .WithMany(p => p.Calibrations)
                .HasForeignKey(ps => ps.PumpEventId);
            modelBuilder.Entity<Calibration>().OwnsOne(o => o.BG);



            modelBuilder.Entity<EventType>().HasKey(e => e.EventTypeId);
            modelBuilder.Entity<EventType>().Property(c => c.EventTypeId).ValueGeneratedNever();
            modelBuilder.Entity<EventType>().ToTable("EventType");


            modelBuilder.Entity<PumpEventAlert>().HasKey(e => e.PumpEventAlertId);
            modelBuilder.Entity<PumpEventAlert>().Property(c => c.PumpEventAlertId).ValueGeneratedNever();
            modelBuilder.Entity<PumpEventAlert>().ToTable("PumpEventAlert");

            modelBuilder.Entity<SensorReadingAlert>().HasKey(e => e.SensorReadingAlertId);
            modelBuilder.Entity<SensorReadingAlert>().Property(c => c.SensorReadingAlertId).ValueGeneratedNever();
            modelBuilder.Entity<SensorReadingAlert>().ToTable("SensorReadingAlert");

            
        }



        public override int SaveChanges()
        {
            //workaround, as OwnsOne do support nullables
            foreach (var entry in ChangeTracker.Entries()
                  .Where(e => e.Entity is PumpStatus && e.State == EntityState.Added))
            {
                if (entry.Entity is PumpStatus)
                {
                    if (entry.Reference("AlertDateTime").CurrentValue == null)
                    {
                        entry.Reference("AlertDateTime").CurrentValue = new DateTimeDataType();
                    }
                    if (entry.Reference("SgvDateTime").CurrentValue == null)
                    {
                        entry.Reference("SgvDateTime").CurrentValue = new DateTimeDataType();
                    }
                }

            }
            return base.SaveChanges();
        }
    }
}
