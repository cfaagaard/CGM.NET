using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace CGM.Communication.Data.Repository
{
    public class BaseRepository<T> where T : class, new()
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<BaseRepository<T>>();
        //protected SQLiteConnection _connection;
        protected CgmUnitOfWork _uow;
        public BaseRepository(CgmUnitOfWork uow)
        {
            _uow = uow;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            var s = _uow.Connection.InsertAll(entities);

        }

        public List<T> ToList()
        {
            return _uow.Connection.Table<T>().ToList();
        }

        public void Add(T entity)
        {
            var s = _uow.Connection.Insert(entity);
        }

        public virtual void Update(T entity)
        {
            var s = _uow.Connection.Update(entity);
        }

        public void Remove(T entity)
        {
            var s = _uow.Connection.Delete(entity);
        }

        public void Clear()
        {
            var s = _uow.Connection.DeleteAll<T>();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _uow.Connection.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ExportAsJson()
        {
            string filename = $"{typeof(T).Name}_{DateTime.Now.ToString("yyyyMMddhhmm")}.json";
            string path = "Exported";
            string fullpath = System.IO.Path.Combine(path, filename);
            ExportAsJson(fullpath);
        }

        public void ExportAsJson(string path)
        {
            var list = _uow.Connection.Table<T>().ToList();
            var json = JsonConvert.SerializeObject(list);
            System.IO.File.WriteAllText(path, json);
        }

        public List<T> ReadFromJson(string path)
        {
            string json = System.IO.File.ReadAllText(path);
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
            return list;
        }

        public void ImportJson(string path)
        {
            var list = ReadFromJson(path);
            _uow.Connection.InsertAll(list);

        }

        internal void ReCreateTable(string exportPath)
        {
            string tablename = typeof(T).Name;


            var cmd = _uow.Connection.CreateCommand($"SELECT count(*) FROM sqlite_master WHERE name ='{tablename}' and type='table'");
            var count = cmd.ExecuteScalar<int>();
            if (count==0)
            {
                _uow.Connection.CreateTable<T>();
            }
            else
            {
                string filename = $"Temp{tablename}.json";
                string fullpath = System.IO.Path.Combine(exportPath, filename);
                ExportAsJson(fullpath);
                _uow.Connection.DropTable<T>();
                _uow.Connection.CreateTable<T>();
                ImportJson(fullpath);
                System.IO.File.Delete(fullpath);
            }


        }

    }
}
