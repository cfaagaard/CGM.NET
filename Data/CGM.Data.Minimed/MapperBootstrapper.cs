using AutoMapper;
using CGM.Communication;
using CGM.Communication.MiniMed.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace CGM.Data.Minimed
{
    public class MapperBootstrapper
    {
        public MapperBootstrapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(typeof(Model.PumpEventDataProfile));
                //cfg.CreateMap<DateTimeDataType, Model.Owned.>().ConvertUsing(s=>s.DateTime.Value);
                cfg.CreateMap<BgDataType, Model.Owned.BgDataType>().ConvertUsing(s => new Model.Owned.BgDataType() { BG = s.BG, BG_RAW = s.BG_RAW });
                cfg.CreateMap<SgDataType, Model.Owned.SgDataType>().ConvertUsing(s => new Model.Owned.SgDataType() { SG = s.SG, SG_RAW = s.SG_RAW });
                cfg.CreateMap<InsulinDataType, Model.Owned.InsulinDataType>().ConvertUsing(s => new Model.Owned.InsulinDataType() { Insulin = s.Insulin, InsulinRaw = s.InsulinRaw });

                //cfg.CreateMap<DateTimeDataType, Model.Owned.DateTimeDataType>().ConvertUsing(s => new Model.Owned.DateTimeDataType() { Date = s.DateTime.Value, Rtc = s.Rtc, Offset = s.Offset,DateTimeEpoch=s.DateTimeEpoch });

                cfg.CreateMap<DateTimeDataType, Model.Owned.DateTimeDataType>()
                .ConvertUsing(s =>
                {
                    if (s != null && s.DateTime.HasValue)
                    {
                        return new Model.Owned.DateTimeDataType() { Date = s.DateTime.Value, Rtc = s.Rtc, Offset = s.Offset, DateTimeEpoch = s.DateTimeEpoch };
                    }
                    else
                    {
                        return new Model.Owned.DateTimeDataType();
                    }
                }
                );

            });
        }
    }
}
