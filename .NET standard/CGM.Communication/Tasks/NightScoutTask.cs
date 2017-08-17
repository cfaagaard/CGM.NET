using CGM.Communication.Common.Serialize;
using CGM.Communication.Data;
using CGM.Communication.Data.Repository;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Tasks
{
    public class NightScoutTask : ITask
    {
        protected CancellationTokenSource _cts;
        protected int _delayInSeconds = 150;
        private Setting _setting;
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
            using (Data.Repository.CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                _setting= uow.Setting.GetSettings();
                Intervalseconds = _setting.OtherSettings.IntervalSeconds;
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
                    session = await uow.Pump.GetPumpDataAndUploadAsync(_device, GetBattery(), _token);

                    //session = await uow.Pump.GetPumpSessionAsync(_device,_token);
                    if (session != null)
                    {
                        GotSession(session);
                    }

                    if (this.timer != null)
                    {
                        timer.Dispose();
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
                        SetUpTimer(session.NextRun.Value);
                        session.NewSession();
                    }

                }

            }

        }

        protected virtual void GotSession(SerializerSession session)
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
