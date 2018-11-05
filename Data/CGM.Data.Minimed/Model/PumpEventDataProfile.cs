using AutoMapper;
using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.DataTypes;

namespace CGM.Data.Minimed.Model
{
    public class PumpEventDataProfile : Profile
    {
        public PumpEventDataProfile()
        {
            //pump
            CreateMap<DeviceCharacteristicsResponse, Model.Pump>()
               .ForMember(dest => dest.Mac, opt => opt.MapFrom(src => BitConverter.ToString(src.Mac)));

            //BayerStick
            CreateMap<BayerStickInfoResponse, Model.BayerStick>()
                .ForMember(dest => dest.AccessPassword, opt => opt.MapFrom(src => src.Reader.AccessPassword))
                .ForMember(dest => dest.MeterLanguage, opt => opt.MapFrom(src => src.Reader.MeterLanguage))
                .ForMember(dest => dest.TestReminderInterval, opt => opt.MapFrom(src => src.Reader.TestReminderInterval))

              .ForMember(dest => dest.DigitalEngineVersion, opt => opt.MapFrom(src => src.Reader.DeviceVersion.DigitalEngineVersion))
              .ForMember(dest => dest.GameBoardVersion, opt => opt.MapFrom(src => src.Reader.DeviceVersion.GameBoardVersion))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Reader.DeviceVersion.Name))
              .ForMember(dest => dest.ModelNumber, opt => opt.MapFrom(src => src.Reader.DeviceVersion.ModelNumber))
              .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Reader.DeviceVersion.Manufacturer))
              .ForMember(dest => dest.RFID, opt => opt.MapFrom(src => src.Reader.DeviceVersion.RFID))
              .ForMember(dest => dest.SerialNum, opt => opt.MapFrom(src => src.Reader.DeviceVersion.SerialNum))
              .ForMember(dest => dest.SerialNumSmall, opt => opt.MapFrom(src => src.Reader.DeviceVersion.SerialNumSmall))
              .ForMember(dest => dest.SkuIdentifier, opt => opt.MapFrom(src => src.Reader.DeviceVersion.SkuIdentifier))
              ;



            //PumpEvent
            CreateMap<BasePumpEvent, Model.PumpEvent>()
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ToString()))
            .ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => src.EventTypeRaw))
            .ForMember(dest=> dest.EventType,opt=>opt.Ignore());
            //SensorEvent
            CreateMap<BasePumpEvent, Model.SensorEvent>()
        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ToString()))
        .ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => src.EventTypeRaw))
        .ForMember(dest => dest.EventType, opt => opt.Ignore());

            //SgReading
            CreateMap<SENSOR_GLUCOSE_READINGS_EXTENDED_Detail, SensorReading>()
                    .ForMember(dest => dest.ReadingDateTime, opt => opt.MapFrom(src => src.EventDate.DateTime));

            //PumpStatus
            CreateMap<PumpStatusMessage, PumpStatus>()

                   .ForMember(dest => dest.PS_BolusingDual, opt => opt.MapFrom(src => src.Status.BolusingDual))
                   .ForMember(dest => dest.PS_BolusingNormal, opt => opt.MapFrom(src => src.Status.BolusingNormal))
                   .ForMember(dest => dest.PS_BolusingSquare, opt => opt.MapFrom(src => src.Status.BolusingSquare))
                   .ForMember(dest => dest.PS_CgmActive, opt => opt.MapFrom(src => src.Status.CgmActive))
                   .ForMember(dest => dest.PS_DeliveringInsulin, opt => opt.MapFrom(src => src.Status.DeliveringInsulin))
                   .ForMember(dest => dest.PS_Suspended, opt => opt.MapFrom(src => src.Status.Suspended))
                   .ForMember(dest => dest.PS_TempBasalActive, opt => opt.MapFrom(src => src.Status.TempBasalActive))
                   .ForMember(dest => dest.PS_BolusingDual, opt => opt.MapFrom(src => src.Status.BolusingDual))


                   .ForMember(dest => dest.SS_Calibrating, opt => opt.MapFrom(src => src.SensorStatus.Calibrating))
                   .ForMember(dest => dest.SS_CalibrationComplete, opt => opt.MapFrom(src => src.SensorStatus.CalibrationComplete))
                   .ForMember(dest => dest.SS_Exception, opt => opt.MapFrom(src => src.SensorStatus.Exception))

                   .ForMember(dest => dest.ActiveInsulin, opt => opt.MapFrom(src => src.ActiveInsulin.Insulin))
                   .ForMember(dest => dest.ActiveInsulinRaw, opt => opt.MapFrom(src => src.ActiveInsulin.InsulinRaw))

      
                   .ForMember(dest => dest.AlertName, opt => opt.MapFrom(src => src.AlertName.ToString()));




            //dailytotal
            CreateMap<DAILY_TOTALS_Event, DailyTotal>();

            CreateMap<CALIBRATION_COMPLETE_Event, Calibration>();
            
        }
    }
}
