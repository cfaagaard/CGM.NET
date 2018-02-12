using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using CGM.Communication.Log;


namespace  CMG.Data.Sqlite.Repository
{
    public class CgmUnitOfWork : IDisposable
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<BaseRepository<CgmUnitOfWork>>();
        private int _currentDBVersion = 1;
        public static string DatabaseName { get { return @"CgmData.db"; } }

        private SQLiteConnection _connection;

        public SQLiteConnection Connection { get { return _connection; } }

        public DeviceRepository Device { get; set; }
        public SettingRepository Setting { get; set; }
        //public NightscoutRepository Nightscout { get; set; }
        //public PumpRepository Pump { get; set; }
        //public CommunicationMessageRepository CommunicationMessage { get; set; }
        public HistoryStatusRepository HistoryStatus { get; set; }


        public HistoryRepository History { get; set; }

        public CgmUnitOfWork(string databasePath)
        {
           string path= Path.Combine(databasePath, DatabaseName);
            _connection = new SQLiteConnection(path);

            Device = new DeviceRepository(this);
            Setting = new SettingRepository(this);
            //Nightscout = new NightscoutRepository(this);
            //Pump = new PumpRepository(this);
            //CommunicationMessage = new CommunicationMessageRepository(this);
            History = new HistoryRepository(this);
            HistoryStatus = new HistoryStatusRepository(this);
        }

        public CgmUnitOfWork():this("")
        {

        }

        public void CreateTables()
        {

            _connection.CreateTable<Device>();
            _connection.CreateTable<SqliteSetting>();
            _connection.CreateTable<History>();
            _connection.CreateTable<HistoryStatus>();

            var setting=Setting.GetSettings();
            setting.DatabaseVersion = 1;
            Setting.Update(setting);
            //string sql = "INSERT INTO SqliteSetting(SettingId,DatabaseVersion) VALUES(1);";

            //var command = _connection.CreateCommand(sql);
            //var count = command.ExecuteNonQuery();

        }

        private void ReCreateTables(string exportPath)
        {
            this.Device.ReCreateTable(exportPath);
            this.Setting.ReCreateTable(exportPath);
            this.History.ReCreateTable(exportPath);
            this.HistoryStatus.ReCreateTable(exportPath);
        }

        public void CheckDatabaseVersion(string exportPath)
        {
            string sql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='SqliteSetting';";

           var command= _connection.CreateCommand(sql);
            var count=command.ExecuteScalar<int>();
            if (count==0)
            {
                CreateTables();
            }
            var settings = this.Setting.GetSettings();


            if (settings.DatabaseVersion < _currentDBVersion)
            {
                if (settings.DatabaseVersion != 0)
                {
                    ReCreateTables(exportPath);
                }
                settings.DatabaseVersion = _currentDBVersion;
                this.Setting.Update(settings);
                Logger.LogInformation($"Local database updated to version {_currentDBVersion}");
            }




        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _connection.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
