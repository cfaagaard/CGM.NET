using AutoMapper;
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
            cfg.AddProfile(typeof(Model.PumpEventDataProfile))
            
            );
        }
    }
}
