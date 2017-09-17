
using CGM.Communication.Data;
using CGM.Communication.Data.Nightscout;
using CGM.Communication.Data.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Model;
using CGM.Communication.Common.Serialize;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Interfaces;
using CGM.Communication.MiniMed.DataTypes;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;

namespace CGM.Communication.Data.Repository
{
    public class PumpRepository
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<PumpRepository>();
        private CgmUnitOfWork _uow;
        public PumpRepository(CgmUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SerializerSession> GetPumpSessionAsync(IDevice device, CancellationToken cancelToken)
        {
            if (device == null)
            {
                throw new ArgumentException("No device found");
            }
            MiniMed.MiniMedContext context = new MiniMed.MiniMedContext(device);

            return await context.GetPumpSessionAsync(cancelToken);

        }

        public async Task<SerializerSession> GetOnlyCnlSessionAsync(IDevice device, CancellationToken cancelToken)
        {
            if (device == null)
            {
                throw new ArgumentException("No device found");
            }
            MiniMed.MiniMedContext context = new MiniMed.MiniMedContext(device);

            return await context.GetOnlyCnlSessionAsync(cancelToken);

        }



        public async Task<SerializerSession> GetPumpDataAndUploadAsync(IDevice device, int uploaderBattery, CancellationToken cancelToken)
        {
            try
            {
                SerializerSession session = await GetPumpSessionAsync(device, cancelToken);
                if (session != null)
                {
                    session.UploaderBattery = uploaderBattery;
                    if (!cancelToken.IsCancellationRequested && session.CanSaveSession)
                    {
                        await SaveSession(session);
                        _uow.History.SaveHistory(session);

                        if (session.RadioChannel != 0x00)
                        {
                            try
                            {
                                await _uow.Nightscout.UploadToNightScout(session, cancelToken).TimeoutAfter(15000);
                            }
                            catch (Exception ex)
                            {
                                //no local internet connection
                                //no connection to azure website
                                Logger.LogError($"Error in upload: {ex.Message}");
                            }

                        }
                    }
                }
                return session;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task SaveSession(SerializerSession session)
        {
            _uow.Device.AddUpdateSessionToDevice(session);
        }



    }
}
