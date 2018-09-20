using CGM.Data.Minimed.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=minimed.db");
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pump>().HasKey(e => e.PumpId);
            modelBuilder.Entity<Pump>().Property(c => c.PumpId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Pump>().ToTable("Pump");

            modelBuilder.Entity<BayerStick>().HasKey(e => e.BayerStickId);
            modelBuilder.Entity<BayerStick>().Property(c => c.BayerStickId).ValueGeneratedOnAdd();
            modelBuilder.Entity<BayerStick>().ToTable("BayerStick");

            modelBuilder.Entity<PumpStatus>().HasKey(e => e.PumpStatusId);
            modelBuilder.Entity<PumpStatus>().Property(c => c.PumpStatusId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PumpStatus>()
                .HasOne<Pump>(ps => ps.Pump)
                .WithMany(p => p.PumpStatus)
                .HasForeignKey(ps => ps.PumpId);
            modelBuilder.Entity<PumpStatus>().ToTable("PumpStatus");

            modelBuilder.Entity<SensorEvent>().HasKey(e => e.SensorEventId);
            modelBuilder.Entity<SensorEvent>().Property(c => c.SensorEventId).ValueGeneratedOnAdd();
            modelBuilder.Entity<SensorEvent>().ToTable("SensorEvent");
            modelBuilder.Entity<SensorEvent>()
               .HasOne<Pump>(ps => ps.Pump)
               .WithMany(p => p.SensorEvents)
               .HasForeignKey(ps => ps.PumpId);

            modelBuilder.Entity<PumpEvent>().HasKey(e => e.PumpEventId);
            modelBuilder.Entity<PumpEvent>().Property(c => c.PumpEventId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PumpEvent>().ToTable("PumpEvent");
            modelBuilder.Entity<PumpEvent>()
               .HasOne<Pump>(ps => ps.Pump)
               .WithMany(p => p.PumpEvents)
               .HasForeignKey(ps => ps.PumpId);

            modelBuilder.Entity<PumpEvent>()
             .HasOne<BayerStick>(ps => ps.BayerStick)
             .WithMany(p => p.PumpEvents)
             .HasForeignKey(ps => ps.BayerStickId);


            modelBuilder.Entity<SensorReading>().HasKey(e => e.SensorReadingId);
            modelBuilder.Entity<SensorReading>().Property(c => c.SensorReadingId).ValueGeneratedOnAdd();
            modelBuilder.Entity<SensorReading>().ToTable("SensorReading");
            modelBuilder.Entity<SensorReading>()
                .HasOne<SensorEvent>(ps => ps.SensorEvent)
                .WithMany(p => p.SensorReadings)
                .HasForeignKey(ps => ps.SensorEventId);

            //not supported - complex types 
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.AccessPassword).HasColumnName("AccessPassword");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.HMACbyte).HasColumnName("HMACbyte");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.MeterLanguage).HasColumnName("MeterLanguage");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.TestReminderInterval).HasColumnName("TestReminderInterval");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.AnalogEngineVersion).HasColumnName("AnalogEngineVersion");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.DigitalEngineVersion).HasColumnName("DigitalEngineVersion");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.GameBoardVersion).HasColumnName("GameBoardVersion");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.Manufacturer).HasColumnName("Manufacturer");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.ModelNumber).HasColumnName("ModelNumber");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.Name).HasColumnName("Name");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.RFID).HasColumnName("RFID");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.SerialNum).HasColumnName("SerialNum");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.SerialNumSmall).HasColumnName("SerialNumSmall");
            //modelBuilder.Entity<BayerStickInfoResponse>().Property(e => e.Reader.DeviceVersion.SkuIdentifier).HasColumnName("SkuIdentifier");


            //modelBuilder.Entity<PumpEvent>().HasKey(e => e.Key);
            //modelBuilder.Entity<PumpEvent>().Property(p => p.Key).HasColumnName("Key");
            //modelBuilder.Entity<PumpEvent>().Property(p => p.BytesAsString).HasColumnName("BytesAsString");
            //modelBuilder.Entity<PumpEvent>().Property(p => p.EventDate.).HasColumnName("BytesAsString");
            //modelBuilder.Entity<PumpEvent>().Property(p => p.BytesAsString).HasColumnName("BytesAsString");
            //modelBuilder.Entity<PumpEvent>().Property(p => p.BytesAsString).HasColumnName("BytesAsString");
            //modelBuilder.Entity<PumpEvent>().Property(p => p.BytesAsString).HasColumnName("BytesAsString");

            //modelBuilder.Entity<SENSOR_GLUCOSE_READINGS_EXTENDED_Event>().Property(p=>p.)

            //modelBuilder.Entity<SENSOR_GLUCOSE_READINGS_EXTENDED_Event>().HasKey(e => e.EventDate.Rtc);
            //modelBuilder.Entity<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail>().HasKey(e => e.EventDate.Rtc);
        }
    }
}
