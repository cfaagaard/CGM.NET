using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using CMG.Data.Sqlite.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.LocalClient
{
    public class NightScoutTask : ITask
    {
        protected CancellationTokenSource _cts;
        protected int _delayInSeconds = 150;
        protected Configuration _setting;
        public int Intervalseconds { get; set; }
        //protected int Intervalseconds = 300;

        protected IDevice _device;
        protected ILogger Logger = ApplicationLogging.CreateLogger<NightScoutTask>();
        protected CancellationToken _token;
        protected CancellationTokenSource _tokenSource;
        public TimeSpan Delay { get; protected set; }


        private System.Threading.Timer timer;

        public void Start(IDevice device)
        {
            _device = device;
            Delay = TimeSpan.FromSeconds(_delayInSeconds);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                _setting = uow.Setting.GetSettings();
                Intervalseconds = _setting.IntervalSeconds;
            }

            SetUpTimer(DateTime.Now.AddSeconds(2));
        }

        public void Stop()
        {
            if (this.timer != null)
            {
                timer.Dispose();
            }
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }


        }

        private void SetUpTimer(DateTime runTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = (runTime - current);
            if (timeToGo < TimeSpan.Zero)
            {
                Logger.LogError($"Time passed....");
                return;//time already passed
            }

            this.timer = new System.Threading.Timer(x =>
            {
                try
                {
                    Task.Run(() => this.GetData()).Wait();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error on GetData: {ex.Message} ");
                }
            }

            , null, timeToGo, Timeout.InfiniteTimeSpan);


        }

        private async Task GetData()
        {
            if (!CheckNet())
            {
                Stop();
                return;
            }
            SerializerSession session = null;
            try
            {
                using (CgmUnitOfWork uow = new CgmUnitOfWork())
                {
                    if (_setting.UploadToNightscout)
                    {
                        session = await GetPumpDataAndUploadAsync(_device, GetBattery(), _token);
                    }
                    else
                    {
                        session = await GetPumpSessionAsync(_device, _token);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (!_token.IsCancellationRequested)
                {
                    if (session != null)
                    {
                        if (session.OptimalNextRead.HasValue)
                        {
                            session.NextRun = session.OptimalNextRead.Value;
                            Logger.LogInformation($"Next session: {session.OptimalNextRead.Value} (PumpTime: {session.OptimalNextReadInPumpTime.Value})");
                        }
                        else
                        {
                            var time = DateTime.Now.AddMinutes(5);
                            session.NextRun = time;
                            Logger.LogInformation($"Next session: {time} (From local datetime. No sgv-time)");
                        }
                        if (session != null)
                        {
                            GotSession(session);
                        }
                        if (this.timer != null)
                        {
                            timer.Dispose();
                        }
                        SetUpTimer(session.NextRun.Value);
                        if (session.NeedResetCommunication)
                        {
                            ResetCommunication(session);
                        }
                        session.NewSession();
                    }

                }

            }

        }

        public async Task<SerializerSession> GetPumpSessionAsync(IDevice device, CancellationToken cancelToken)
        {
            if (device == null)
            {
                throw new ArgumentException("No device found");
            }

            IStateRepository stateRepository = new SessionStateRepository();
            SerializerSession session = new SerializerSession();

            MiniMedContext context = new MiniMedContext(device, session, stateRepository);

            return await context.GetPumpSessionAsync(cancelToken);

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
                        using (CgmUnitOfWork uow = new CgmUnitOfWork())
                        {
                            uow.Device.AddUpdateSessionToDevice(session);
                            uow.History.SaveHistory(session);


                            if (session.RadioChannel != 0x00 && session.PumpTime != null)
                            {
                                try
                                {
                                    NightscoutUploadSqliteRestApi upLogic = new NightscoutUploadSqliteRestApi(session);
                                    await upLogic.Upload(cancelToken).TimeoutAfter(15000); 
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError($"Error in upload: {ex.Message}");
                                }

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

        protected virtual void GotSession(SerializerSession session)
        {

        }
        protected virtual void ResetCommunication(SerializerSession session)
        {

        }
        protected virtual bool CheckNet()
        {
            return true;
        }

        protected virtual int GetBattery()
        {
            return 100;
        }

    }
}
