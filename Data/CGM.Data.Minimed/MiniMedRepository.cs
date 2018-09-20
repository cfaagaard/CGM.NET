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

        public void OnStartUp()
        {
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext())
            {
                ctx.Database.Migrate();
            }
        }

        public void DeleteAll()
        {
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext())
            {
                ctx.Database.ExecuteSqlCommand("delete from SensorReading");
                ctx.Database.ExecuteSqlCommand("delete from PumpEvent");
                ctx.Database.ExecuteSqlCommand("delete from SensorEvent");
                ctx.Database.ExecuteSqlCommand("delete from PumpStatus");
                ctx.Database.ExecuteSqlCommand("delete from Pump");
                ctx.Database.ExecuteSqlCommand("delete from BayerStick");

            }
        }

        public Task<int> SaveSession(SerializerSession serializerSession)
        {
           
            using (CGM.Data.Minimed.MinimedContext ctx = new Data.Minimed.MinimedContext())
            {
                //pump

                Pump pump = ctx.Pumps.FirstOrDefault(e => e.SerialNumber == serializerSession.PumpSettings.DeviceCharacteristics.SerialNumber);
                if (pump == null)
                {
                    pump = AutoMapper.Mapper.Map<DeviceCharacteristicsResponse, Pump>(serializerSession.PumpSettings.DeviceCharacteristics);
                    ctx.Pumps.Add(pump);
                }
                else
                {
                    if (!pump.BytesAsString.Equals(serializerSession.PumpSettings.DeviceCharacteristics.BytesAsString))
                    {
                        var updatePump = AutoMapper.Mapper.Map<DeviceCharacteristicsResponse, Pump>(serializerSession.PumpSettings.DeviceCharacteristics);
                        updatePump.PumpId = pump.PumpId;
                        ctx.Pumps.Update(updatePump);
                        pump = updatePump;
                    }
                }



                //bayerstick
                BayerStick bayerStick = ctx.BayerSticks.FirstOrDefault(e => e.SerialNumber == serializerSession.SessionDevice.Device.SerialNumber);
                if (bayerStick == null)
                {
                    bayerStick = AutoMapper.Mapper.Map<BayerStickInfoResponse, BayerStick>(serializerSession.SessionDevice.Device);
                    ctx.BayerSticks.Add(bayerStick);
                }
                else
                {
                    if (!bayerStick.AllBytesAsString.Equals(serializerSession.SessionDevice.Device.AllBytesAsString))
                    {
                        var updateStick = AutoMapper.Mapper.Map<BayerStickInfoResponse, BayerStick>(serializerSession.SessionDevice.Device);
                        updateStick.BayerStickId = bayerStick.BayerStickId;
                        ctx.BayerSticks.Update(updateStick);
                        bayerStick = updateStick;
                    }
                }

                List<Model.PumpEvent> events = new List<Model.PumpEvent>();
                foreach (var pumpEvent in serializerSession.PumpDataHistory.PumpEvents)
                {
                    if (pumpEvent.EventType != EventTypeEnum.PLGM_CONTROLLER_STATE)
                    {
                        //Ignore EventTypeEnum.PLGM_CONTROLLER_STATE. there is a lot and do not know what it is used for.....
                        var pumpeventdata = AutoMapper.Mapper.Map<Communication.MiniMed.Responses.Events.BasePumpEvent, Model.PumpEvent>(pumpEvent);
                        pumpeventdata.BayerStick = bayerStick;
                        pump.PumpEvents.Add(pumpeventdata);
                    }
                }

                foreach (var pumpEvent in serializerSession.PumpDataHistory.SensorEvents)
                {
                    var pumpeventdata = AutoMapper.Mapper.Map<Communication.MiniMed.Responses.Events.BasePumpEvent, Model.SensorEvent>(pumpEvent);

                    pumpeventdata.BayerStick = bayerStick;
                    pump.SensorEvents.Add(pumpeventdata);

                    if (pumpEvent.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED)
                    {
                        var msg = (pumpEvent.Message as SENSOR_GLUCOSE_READINGS_EXTENDED_Event);
                        foreach (var detail in msg.Details)
                        {
                            var reading = Mapper.Map<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail, SensorReading>(detail);
                            pumpeventdata.SensorReadings.Add(reading);
                        }
                    }



                }


                foreach (var item in serializerSession.Status)
                {
                    var status = Mapper.Map<PumpStatusMessage, Model.PumpStatus>(item);
                    status.PumpStatusDateTime = serializerSession.PumpTime.PumpDateTime.Value;
                    pump.PumpStatus.Add(status);
                }

                return ctx.SaveChangesAsync();
            }
        }

        public Task ExecuteTask(SerializerSession session, CancellationToken cancelToken)
        {
            return SaveSession(session);
        }
    }
}
