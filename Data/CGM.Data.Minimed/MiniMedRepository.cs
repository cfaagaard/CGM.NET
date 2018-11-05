using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using CGM.Data.Minimed.Model;
using AutoMapper;
using CGM.Communication.MiniMed.Infrastructur;
using Microsoft.EntityFrameworkCore;
using CGM.Communication.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Data.Minimed
{
    public class MiniMedRepository : ISessionBehavior
    {
        private readonly DbContextOptionsBuilder<MinimedContext> dbContextOptionsBuilder;
        private readonly string dataLoggerKey;

        public MiniMedRepository(DbContextOptionsBuilder<MinimedContext> dbContextOptionsBuilder, string DataLoggerKey)
        {
            this.dbContextOptionsBuilder = dbContextOptionsBuilder;
            dataLoggerKey = DataLoggerKey;
        }

        public void OnStartUp()
        {
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext(dbContextOptionsBuilder.Options))
            {
                ctx.Database.Migrate();
            }
        }

        public void DeleteAll()
        {
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext(dbContextOptionsBuilder.Options))
            {
                ctx.Database.ExecuteSqlCommand("delete from DailyTotal");
                ctx.Database.ExecuteSqlCommand("delete from Calibration");
                ctx.Database.ExecuteSqlCommand("delete from SensorReading");
                ctx.Database.ExecuteSqlCommand("delete from PumpEvent");
                ctx.Database.ExecuteSqlCommand("delete from SensorEvent");
                ctx.Database.ExecuteSqlCommand("delete from PumpStatus");
                ctx.Database.ExecuteSqlCommand("delete from Pump");
                ctx.Database.ExecuteSqlCommand("delete from BayerStick");
                ctx.Database.ExecuteSqlCommand("delete from EventType");
                ctx.Database.ExecuteSqlCommand("delete from PumpEventAlert");
                ctx.Database.ExecuteSqlCommand("delete from SensorReadingAlert");
            }
        }

        public void AddData()
        {
            var eventtypes = Migrations.SeedData.EventType();
            var pumpalerts = Migrations.SeedData.PumpEventAlerts();
            var sensoralerts = Migrations.SeedData.SensorReadingAlerts();
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext(dbContextOptionsBuilder.Options))
            {

                ctx.PumpEventAlerts.AddRange(pumpalerts);
                ctx.EventTypes.AddRange(eventtypes);
                ctx.SensorReadingAlerts.AddRange(sensoralerts);
                ctx.SaveChanges();
            }
        }

        public Task<int> SaveSession(SerializerSession serializerSession)
        {

            Task<int> result = null;
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext(dbContextOptionsBuilder.Options))
            {
                //datalogger
                DataLogger dataLogger = ctx.DataLoggers.FirstOrDefault(e => e.DataLoggerKey == dataLoggerKey);
                if (dataLogger == null)
                {
                    dataLogger = new DataLogger();
                    dataLogger.DataLoggerKey = dataLoggerKey;
                    
                    ctx.DataLoggers.Add(dataLogger);
                    ctx.SaveChanges();
                }


                    //pump

                    Pump pump = ctx.Pumps.FirstOrDefault(e => e.SerialNumber == serializerSession.PumpSettings.DeviceCharacteristics.SerialNumber);
                if (pump == null)
                {
                    pump = AutoMapper.Mapper.Map<DeviceCharacteristicsResponse, Pump>(serializerSession.PumpSettings.DeviceCharacteristics);
                    ctx.Pumps.Add(pump);
                    ctx.SaveChanges();
                }
                else
                {
                    if (!pump.BytesAsString.Equals(serializerSession.PumpSettings.DeviceCharacteristics.BytesAsString))
                    {
                        AutoMapper.Mapper.Map<DeviceCharacteristicsResponse, Pump>(serializerSession.PumpSettings.DeviceCharacteristics, pump);
                        ctx.Pumps.Update(pump);
                        ctx.SaveChanges();
                    }
                }



                //bayerstick
                BayerStick bayerStick = ctx.BayerSticks.FirstOrDefault(e => e.SerialNumber == serializerSession.SessionDevice.Device.SerialNumber);
                if (bayerStick == null)
                {
                    bayerStick = AutoMapper.Mapper.Map<BayerStickInfoResponse, BayerStick>(serializerSession.SessionDevice.Device);
                    ctx.BayerSticks.Add(bayerStick);
                    ctx.SaveChanges();
                }

                //dataloggerreading
                DataLoggerReading loggerReading = new DataLoggerReading();
                loggerReading.DataLogger = dataLogger;
                loggerReading.BayerStick = bayerStick;

                if (serializerSession.PumpTime.PumpDateTime.HasValue)
                {
                    loggerReading.ReadingDateTime = serializerSession.PumpTime.PumpDateTime.Value;
                }
                else
                {
                    loggerReading.ReadingDateTime = DateTime.Now;
                }

                if (serializerSession.SessionSystem.NextRun.HasValue)
                {
                    loggerReading.NextReadingDateTime = serializerSession.SessionSystem.NextRun.Value;
                }

                List<Model.PumpEvent> events = new List<Model.PumpEvent>();
                DateTime maxPumpEvents = new DateTime(2000, 1, 1);
                if (ctx.PumpEvents.Count(e => e.PumpId == pump.PumpId) > 0)
                {
                    maxPumpEvents = ctx.PumpEvents.Where(e => e.PumpId == pump.PumpId).Max(e => e.EventDate.Date);
                }

                //Ignore EventTypeEnum.PLGM_CONTROLLER_STATE. there is a lot and do not know what it is used for.....
                var PumpEventsToSave = serializerSession.PumpDataHistory.PumpEvents.Where(e =>
                    e.EventDate.DateTime > maxPumpEvents &&
                    e.EventType != EventTypeEnum.PLGM_CONTROLLER_STATE).ToList();

                foreach (var pumpEvent in PumpEventsToSave)
                {
                    var pumpeventdata = AutoMapper.Mapper.Map<Communication.MiniMed.Responses.Events.BasePumpEvent, Model.PumpEvent>(pumpEvent);
                    pumpeventdata.DataLoggerReading = loggerReading;
                    pump.PumpEvents.Add(pumpeventdata);

                    if (pumpEvent.EventType == EventTypeEnum.DAILY_TOTALS)
                    {
                        var msg = (pumpEvent.Message as DAILY_TOTALS_Event);
                        var DAILY_TOTAL = Mapper.Map<DAILY_TOTALS_Event, DailyTotal>(msg);
                        pumpeventdata.DailyTotals.Add(DAILY_TOTAL);
                    }

                    if (pumpEvent.EventType == EventTypeEnum.CALIBRATION_COMPLETE)
                    {
                        var msg = (pumpEvent.Message as CALIBRATION_COMPLETE_Event);
                        var calibration = Mapper.Map<CALIBRATION_COMPLETE_Event, Calibration>(msg);
                        pumpeventdata.Calibrations.Add(calibration);
                    }

                    if (pumpEvent.EventType == EventTypeEnum.ALARM_NOTIFICATION)
                    {
                        var msg = (pumpEvent.Message as ALARM_NOTIFICATION_Event);
                        pumpeventdata.PumpEventAlertId = msg.AlarmType;
                    }
                    if (pumpEvent.EventType == EventTypeEnum.ALARM_CLEARED)
                    {
                        var msg = (pumpEvent.Message as ALARM_CLEARED_Event);
                        pumpeventdata.PumpEventAlertId = msg.AlarmType;
                    }
                }
                ctx.SaveChanges();


                DateTime maxSensorEvents = new DateTime(2000, 1, 1);
                if (ctx.SensorEvents.Count(e => e.PumpId == pump.PumpId) > 0)
                {
                    maxSensorEvents = ctx.SensorEvents.Where(e => e.PumpId == pump.PumpId).Max(e => e.EventDate.Date);
                }

                var SensorEventsToSave = serializerSession.PumpDataHistory.SensorEvents.Where(e => e.EventDate.DateTime > maxSensorEvents);
                foreach (var pumpEvent in SensorEventsToSave)
                {
                    var pumpeventdata = AutoMapper.Mapper.Map<Communication.MiniMed.Responses.Events.BasePumpEvent, Model.SensorEvent>(pumpEvent);

                    pumpeventdata.DataLoggerReading = loggerReading;
                    pump.SensorEvents.Add(pumpeventdata);
                    if (pumpEvent.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED)
                    {
                        var msg = (pumpEvent.Message as SENSOR_GLUCOSE_READINGS_EXTENDED_Event);
                        foreach (var detail in msg.Details)
                        {
                            var reading = Mapper.Map<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail, SensorReading>(detail);
                            if (reading.Amount > 700)
                            {

                                reading.SensorReadingAlertId = reading.Amount.Value;
                                reading.Amount = null;
                            }
                            if (reading.PredictedSg>400)
                            {
                                reading.PredictedSg_Alert = reading.PredictedSg;
                                reading.PredictedSg = null;
                            }
                            pumpeventdata.SensorReadings.Add(reading);
                        }
                    }



                }

                var keys=ctx.SensorReadings.Local.GroupBy(e => e.SensorReadingAlertId).Select(e => e.Key);



                ctx.SaveChanges();
                var orderlist = serializerSession.Status.OrderBy(e => e.PumpStatusDateTime);
                foreach (var item in orderlist)
                {
                    var status = Mapper.Map<PumpStatusMessage, Model.PumpStatus>(item);
                    //status.PumpStatusDateTime = serializerSession.PumpTime.PumpDateTime.Value;
                    pump.PumpStatus.Add(status);
                }
                ctx.SaveChanges();

                result = ctx.SaveChangesAsync();
            }
            return result;
        }

        public Task ExecuteTask(SerializerSession session, CancellationToken cancelToken)
        {
            return SaveSession(session);
        }
    }
}
