using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using CGM.Data.Model;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.Common;
using CGM.Communication.MiniMed;

namespace CGM.Data
{
    public class SessionStateRepository : CGM.Communication.Interfaces.IStateRepository
    {
        public void AddKeys(List<string> keys)
        {
            List<HistoryStatus> statusList = new List<HistoryStatus>();
            keys.ForEach(e => statusList.Add(new HistoryStatus() { Key = e, Status = 0 }));
            using (CgmContext ctx = new CgmContext())
            {
                
                try
                {
                    ctx.HistoryStatus.AddRange(statusList);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw;
                }
               
            }

        }

        public void AddUpdateSessionToDevice(SerializerSession session)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                Device dev = cgmContext.Device.FirstOrDefault(e => e.SerialNumber == session.SessionDevice.Device.SerialNumber);
                if (dev == null)
                {
                    dev = new Device();
                    dev.Name = session.SessionDevice.Device.ModelNumber;
                    dev.SerialNumber = session.SessionDevice.Device.SerialNumber;
                    dev.SerialNumberFull = session.SessionDevice.Device.SerialNumberFull;
                    cgmContext.Device.Add(dev);
                }

                if (session.SessionCommunicationParameters.LinkMac != null)
                {
                    dev.LinkMac = BitConverter.ToString(session.SessionCommunicationParameters.LinkMac);
                }
                if (session.SessionCommunicationParameters.PumpMac != null)
                {
                    dev.PumpMac = BitConverter.ToString(session.SessionCommunicationParameters.PumpMac);
                }
                if (session.SessionCommunicationParameters.LinkKey != null)
                {
                    dev.LinkKey = BitConverter.ToString(session.SessionCommunicationParameters.LinkKey);
                }

                dev.RadioChannel = session.SessionCommunicationParameters.RadioChannel.ToString();


                cgmContext.SaveChanges();
            }

        }

        public List<BasePumpEvent> GetHistoryWithNoStatus(List<int> eventFilter, SerializerSession session)
        {


            string sql = "select H.* from History H Left Outer join (select key from historystatus)  HS on H.Key = HS.Key WHERE H.EventType IN ({0})  AND HS.Key is null";
            string sqlExe = string.Format(sql, string.Join(",", eventFilter));
            using (CgmContext ctx = new CgmContext())
            {
                var NewEvents = ctx.History.FromSql(sqlExe).ToList();

                return Convert(NewEvents, session);
            }

        }

        public List<BasePumpEvent> GetHistoryWithNoStatus(SerializerSession session)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                string sql = "select H.* from History H Left Outer join (select key from historystatus)  HS on H.Key = HS.Key WHERE HS.Key is null";
                var NewEvents = cgmContext.History.FromSql(sql).ToList();

                return Convert(NewEvents, session);
            }
        }

        public void GetOrSetSessionAndSettings(SerializerSession session)
        {
            if (!string.IsNullOrEmpty(session.SessionDevice.Device.SerialNumber))
            {
                using (CgmContext cgmContext = new CgmContext())
                {
                    Device device = cgmContext.Device.FirstOrDefault(e => e.SerialNumber == session.SessionDevice.Device.SerialNumber);
                    if (device != null)
                    {
                        if (!string.IsNullOrEmpty(device.LinkMac))
                        {
                            session.SessionCommunicationParameters.LinkMac = device.LinkMac.GetBytes();
                        }
                        if (!string.IsNullOrEmpty(device.LinkKey))
                        {
                            session.SessionCommunicationParameters.LinkKey = device.LinkKey.GetBytes();
                        }

                        if (!string.IsNullOrEmpty(device.PumpMac))
                        {
                            session.SessionCommunicationParameters.PumpMac = device.PumpMac.GetBytes();
                        }

                        session.SessionCommunicationParameters.RadioChannel = byte.Parse(device.RadioChannel);
                        session.SessionDevice.Device.ModelNumber = device.Name;
                        session.SessionDevice.Device.SerialNumber = device.SerialNumber;
                        session.SessionDevice.Device.SerialNumberFull = device.SerialNumberFull;

                    }
                    else
                    {
                        //Insert the device
                        Device dev = new Device();
                        dev.Name = session.SessionDevice.Device.ModelNumber;
                        dev.SerialNumber = session.SessionDevice.Device.SerialNumber;
                        dev.SerialNumberFull = session.SessionDevice.Device.SerialNumberFull;
                        cgmContext.Device.Add(dev);
                        cgmContext.SaveChanges();
                    }
                }
            }
        }

        public void SaveConfiguration<T>(T configuration)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                var config = cgmContext.GetConfiguration<T>();
                if (config != null)
                {
                    cgmContext.UpdateConfiguration<T>(configuration);
                }
                else
                {
                    cgmContext.AddConfiguration<T>(configuration);
                }
            }
        }

        public T GetConfiguration<T>()
        {
            using (CgmContext cgmContext = new CgmContext())
            {
               return cgmContext.GetConfiguration<T>();

            }
        }

        public void SaveSession(SerializerSession session)
        {

            this.AddUpdateSessionToDevice(session);
            using (CgmContext cgmContext = new CgmContext())
            {
                var settings = cgmContext.GetConfiguration<PumpSettings>();
                if (settings != null)
                {
                    cgmContext.UpdateConfiguration<PumpSettings>(session.PumpSettings);
                }
                else
                {
                    cgmContext.AddConfiguration<PumpSettings>(session.PumpSettings);
                }

                if (session.PumpDataHistory != null)
                {
                    var all = session.PumpDataHistory.PumpEvents.Where(e => e.EventType == EventTypeEnum.PLGM_CONTROLLER_STATE).ToList();
                    all.ForEach(e => session.PumpDataHistory.PumpEvents.Remove(e));

                    if (session.PumpDataHistory.PumpEvents.Count > 0)
                    {
                        this.Sync(session.PumpDataHistory.PumpEvents.Select(e => new History(e)).ToList(), (int)HistoryDataTypeEnum.Pump);
                    }
                    if (session.PumpDataHistory.SensorEvents.Count > 0)
                    {
                        this.Sync(session.PumpDataHistory.SensorEvents.Select(e => new History(e)).ToList(), (int)HistoryDataTypeEnum.Sensor);
                    }

                    SaveLastReadHistoryInConfiguration();
                }

                if (session.Status != null && session.Status.Count > 0 && session.PumpTime.PumpDateTime.HasValue)
                {
                    foreach (var item in session.Status)
                    {
                        DeviceStatus deviceStatus = new DeviceStatus();
                        deviceStatus.DeviceStatusKey = session.PumpTime.Rtc.GetInt32(0).ToString();
                        deviceStatus.DeviceStatusBytes = item.BytesAsString;
                        if (cgmContext.DeviceStatus.FirstOrDefault(e => e.DeviceStatusKey == deviceStatus.DeviceStatusKey) != null)
                        {
                            cgmContext.DeviceStatus.Add(deviceStatus);
                            cgmContext.SaveChanges();
                        }

                    }
                }
            }
        }

        public List<BasePumpEvent> GetHistory(List<int> eventFilter, SerializerSession session)
        {
            return GetHistory($" EventType IN ({string.Join(",", eventFilter)})", session);
        }

        private List<BasePumpEvent> Convert(List<History> histories, SerializerSession session)
        {
            List<BasePumpEvent> eventsToHandle = new List<BasePumpEvent>();
            if (histories.Count > 0)
            {
                Serializer serializer = new Serializer(session);

                foreach (var item in histories)
                {
                    var pumpevent = serializer.Deserialize<BasePumpEvent>(item.HistoryBytes.GetBytes());
                    pumpevent.HistoryDataType = item.HistoryDataType;
                    eventsToHandle.Add(pumpevent);
                }


            }
            return eventsToHandle;
        }

        public void Sync(List<History> histories, int datatype)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                var query = cgmContext.History.Where(e => e.HistoryDataType == datatype).ToList();


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

                    cgmContext.History.AddRange(missingHistory);
                    cgmContext.SaveChanges();
                }
                //keep only the last 5 days (ca. 5*400 =2000 events ish)
                Delete(2000, datatype);
            }
        }

        private void SaveLastReadHistoryInConfiguration()
        {
            var values = Enum.GetValues(typeof(HistoryDataTypeEnum)).Cast<HistoryDataTypeEnum>();
            using (CgmContext cgmContext = new CgmContext())
            {
                var minimedConfiguration = cgmContext.GetConfiguration<MinimedConfiguration>();

                if (minimedConfiguration == null)
                {
                    minimedConfiguration = new MinimedConfiguration();
                    cgmContext.AddConfiguration<MinimedConfiguration>(minimedConfiguration);
                }

                minimedConfiguration.LastRead = new List<LastPumpRead>();
                foreach (var value in values)
                {
                    int rtc = GetMaxRtc((int)value);
                    if (rtc != 0)
                    {
                        minimedConfiguration.LastRead.Add(new LastPumpRead() { DataType = (int)value, LastRtc = rtc });
                    }


                }

                cgmContext.UpdateConfiguration<MinimedConfiguration>(minimedConfiguration);
            }
        }

        private int GetMaxRtc(int historyDataType)
        {
            int max = 0;
            try
            {
                using (CgmContext cgmContext = new CgmContext())
                {
                    max = cgmContext.History.Where(e => e.HistoryDataType == historyDataType).Max(e => e.Rtc);
                }
            }
            catch (Exception)
            {


            }
            return max;

        }

        private void Delete(int keepCount, int dataType)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                string sql = "Delete from History where HistoryDataType={0} AND key NOT IN (Select key from History where HistoryDataType={0} order by Rtc desc limit {1});";
                string exeSql = string.Format(sql, dataType, keepCount);
                var count = cgmContext.Database.ExecuteSqlCommand(exeSql);
            }
        }

        private List<BasePumpEvent> GetHistory(string where, SerializerSession session)
        {
            using (CgmContext cgmContext = new CgmContext())
            {
                string sql = $"select * from History WHERE {where}";
                var NewEvents = cgmContext.History.FromSql(sql).ToList();
                return Convert(NewEvents, session);
            }
        }
    }
}
