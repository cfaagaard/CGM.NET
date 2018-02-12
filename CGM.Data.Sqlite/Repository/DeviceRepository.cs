
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Extensions;

namespace  CMG.Data.Sqlite.Repository
{
    public class DeviceRepository : BaseRepository<Device>
    {

        public DeviceRepository(CgmUnitOfWork uow) : base(uow)
        {

        }

        public Device GetBySerialNumber(string serialNumber)
        {
            var query = _uow.Connection.Table<Device>().Where(v => v.SerialNumber == serialNumber);
            return query.FirstOrDefault();
        }

        public List<Device> GetAllDevices()
        {
            var query = _uow.Connection.Table<Device>();
            return query.ToList<Device>();
        }

        public bool UpdateOrAdd(Device device)
        {
            var dev = GetBySerialNumber(device.SerialNumber);
            if (dev != null)
            {
                Update(device);
                return false;
            }
            else
            {
                Add(device);
                return true;
            }
        }


        public void GetOrSetSessionAndSettings(SerializerSession session)
        {
            Device device = _uow.Device.GetBySerialNumber(session.Device.SerialNumber);
            if (device != null)
            {
                if (!string.IsNullOrEmpty(device.LinkMac))
                {
                    session.LinkMac = device.LinkMac.GetBytes();
                }
                if (!string.IsNullOrEmpty(device.LinkKey))
                {
                    session.LinkKey = device.LinkKey.GetBytes();
                }

                if (!string.IsNullOrEmpty(device.PumpMac))
                {
                    session.PumpMac = device.PumpMac.GetBytes();
                }


                session.RadioChannel = byte.Parse(device.RadioChannel);

                session.Device.ModelNumber = device.Name;
                session.Device.SerialNumber = device.SerialNumber;
                session.Device.SerialNumberFull = device.SerialNumberFull;

            }
            else
            {
                //Insert the device
                Device dev = MapToDevice(session.Device);
                _uow.Device.Add(dev);
            }

            session.Settings = _uow.Setting.GetSettings();
        }

        public bool AddUpdateSessionToDevice(SerializerSession session)
        {

            Device dev = MapToDevice(session.Device);
            if (session.LinkMac != null)
            {
                dev.LinkMac = BitConverter.ToString(session.LinkMac);
            }
            if (session.PumpMac != null)
            {
                dev.PumpMac = BitConverter.ToString(session.PumpMac);
            }
            if (session.LinkKey != null)
            {
                dev.LinkKey = BitConverter.ToString(session.LinkKey);
            }

            dev.RadioChannel = session.RadioChannel.ToString();


            return _uow.Device.UpdateOrAdd(dev);
        }

        protected Device MapToDevice(BayerStickInfoResponse information)
        {
            Device dev = new Device();
            dev.Name = information.ModelNumber;
            dev.SerialNumber = information.SerialNumber;
            dev.SerialNumberFull = information.SerialNumberFull;
            //dev.FullInfo = information.Value;
            return dev;
        }



    }
}
