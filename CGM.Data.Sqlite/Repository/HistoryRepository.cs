using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SQLite;
using CGM.Communication.Common.Serialize;
using System.Threading.Tasks;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Common;

namespace  CMG.Data.Sqlite.Repository
{


    public class HistoryRepository : BaseRepository<History>
    {
        public HistoryRepository(CgmUnitOfWork uow) : base(uow)
        {
        }

        public List<History> GetHistoryWithNoStatus(List<int> eventTypes)
        {
            string sql = "select H.* from History H Left Outer join (select key from historystatus)  HS on H.Key = HS.Key WHERE H.EventType IN ({0})  AND HS.Key is null";
            string sqlExe = string.Format(sql, string.Join(",", eventTypes));
            return _uow.Connection.Query<History>(sqlExe).ToList();
        }

        public void SaveHistory(SerializerSession session)
        {
            if (session.PumpDataHistory!=null)
            {
                if (session.PumpDataHistory.PumpEvents.Count>0)
                {
                    this.Sync(session.PumpDataHistory.PumpEvents.Select(e => new History(e)).ToList(), (int)HistoryDataTypeEnum.Pump);
                }
                if (session.PumpDataHistory.SensorEvents.Count > 0)
                {
                    this.Sync(session.PumpDataHistory.SensorEvents.Select(e => new History(e)).ToList(), (int)HistoryDataTypeEnum.Sensor);
                    //var last = session.PumpDataHistory.SensorEvents.Last();
                    //session.Settings.LastRead.Add(new LastPumpRead() { DataType = (int)HistoryDataTypeEnum.Sensor, LastRtc = rtc });
                }

                SaveLastReadHistoryInSettings();
            }

        }

        public void Sync(List<History> histories, int datatype)
        {
            var query = _uow.Connection.Table<History>().Where(e => e.HistoryDataType == datatype).ToList();
  

            var SyncQuery =
                   from comp in histories
                   join entry in query on comp.Key equals entry.Key
                   select comp;

            var missingHistory = histories.Except(SyncQuery.ToList()).ToList();

            if (missingHistory.Count > 0)
            {

                //check for double keys. should not happen..... but hey...
                var groups = missingHistory.GroupBy(e => e.Key)
                                    .Select(group => new
                                    {
                                        Metric = group.Key,
                                        Count = group.Count()
                                    });
                var errors = groups.Where(e => e.Count > 1).ToList();
                foreach (var error in errors)
                {
                    var all = missingHistory.Where(e => e.Key == error.Metric).ToList();
                    //keep the first
                    for (int i = 1; i < all.Count; i++)
                    {
                        missingHistory.Remove(all[i]);
                    }

                }

                this.AddRange(missingHistory);

                //keep only the last 5 days (ca. 5*400 =2000 events)
                
            }

            Delete(2000, datatype);
         
        }

        private void Delete(int keepCount, int dataType)
        {
            string sql = "Delete from History where HistoryDataType={0} AND key NOT IN (Select key from History where HistoryDataType={0} order by Rtc desc limit {1});";
            string exeSql = string.Format(sql, dataType, keepCount);
            var count = _uow.Connection.Execute(exeSql);
        }

        public void ResetHistory()
        {
            this.Clear();
            _uow.HistoryStatus.Clear();
            ClearLastReadHistoryInSettings();
        }

        private void ClearLastReadHistoryInSettings()
        {
            var values = Enum.GetValues(typeof(HistoryDataTypeEnum)).Cast<HistoryDataTypeEnum>();

            var settings = _uow.Setting.GetSettings();
            settings.LastRead = new List<LastPumpRead>();
            _uow.Setting.Update(settings);
        }

        private void SaveLastReadHistoryInSettings()
        {
            var values = Enum.GetValues(typeof(HistoryDataTypeEnum)).Cast<HistoryDataTypeEnum>();

            var settings = _uow.Setting.GetSettings();
            settings.LastRead = new List<LastPumpRead>();
            foreach (var value in values)
            {
                int rtc = GetMaxRtc((int)value);
                if (rtc!=0)
                {
                    settings.LastRead.Add(new LastPumpRead() { DataType = (int)value, LastRtc = rtc });
                }
                

            }
            _uow.Setting.Update(settings);
        }

        private int GetMaxRtc(int historyDataType)
        {
            int max = 0;
            try
            {
                max=_uow.Connection.Table<History>().Where(e => e.HistoryDataType == historyDataType).Max(e => e.Rtc);
            }
            catch (Exception)
            {

 
            }
            return max;

        }

    }

}
