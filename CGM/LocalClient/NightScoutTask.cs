using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Common.Serialize.Log;
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
using System.Linq;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Responses.Events;

namespace CGM.LocalClient
{
    public class NightScoutTask : ITask
    {
        protected CancellationTokenSource _cts;
        protected int _delayInSeconds = 150;
        protected Configuration _setting;
        protected int Intervalseconds;

        protected IDevice _device;
        protected ILogger Logger = ApplicationLogging.CreateLogger<NightScoutTask>();
        protected CancellationToken _token;
        protected CancellationTokenSource _tokenSource;
        protected TimeSpan Delay;
        protected SerializerSession session;
        protected IStateRepository stateRepository;
        private System.Threading.Timer timer;

        public void Start(IDevice device)
        {
            if (device == null)
            {
                throw new ArgumentException("No device found");
            }
            session = null;

            _device = device;
            Delay = TimeSpan.FromSeconds(_delayInSeconds);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            stateRepository = new SessionStateRepository();
            session = new SerializerSession();
            stateRepository.GetOrSetSessionAndSettings(session);
            SetConfiguration();
            SetUpTimer(DateTime.Now.AddSeconds(2));
        }

        protected virtual void SetConfiguration()
        {
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                _setting = uow.Setting.GetSettings();
                Intervalseconds = _setting.IntervalSeconds;
            }

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

        protected void SetUpTimer(DateTime runTime)
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

        protected async Task GetData()
        {
            if (!CheckNet())
            {
                Stop();
                return;
            }
            //get data from pump. Fills the session object       
            await GetDataFromPump();
            //get uploaderbattery status
            session.UploaderBattery = GetBattery();
            //save session
            //SaveSession();


            if (_setting.MongoUpload)
            {
                await UploadToMongoDb();
            }

            if (_setting.UploadToNightscout)
            {
                await UploadToNightscout();
            }


        }

        protected async Task UploadToMongoDb()
        {
            if (string.IsNullOrEmpty(session.Settings.MongoDbUrl))
            {
                Logger.LogCritical("Missing MongoDbUrl. Please go to settings and enter the MongoDbUrl.");
            }
            else
            {
                CGM.Data.Mongo.MongoUnitOfWork uow = new Data.Mongo.MongoUnitOfWork(session.Settings.MongoDbUrl);
                uow.SaveSession(session);
            }
            
        }

        protected async Task UploadToNightscout()
        {

            if (session.SessionCommunicationParameters.RadioChannel != 0x00 && session.PumpTime != null)
            {
                try
                {
                    NightscoutUploadSqliteRestApi upLogic = new NightscoutUploadSqliteRestApi(session);
                    await upLogic.Upload(_token).TimeoutAfter(15000);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in upload: {ex.Message}");
                }

            }
            else
            {
                Logger.LogInformation("No data uploaded to Nightscout");
            }
        }

        protected async Task GetDataFromPump()
        {
            try
            {
                MiniMedContext context = new MiniMedContext(_device, session, stateRepository);
                session = await context.GetPumpSessionAsync(_token);
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
                        GotSession(session);

                        if (this.timer != null)
                        {
                            timer.Dispose();
                        }
                        if (session.SessionSystem.NextRun.HasValue)
                        {
                            SetUpTimer(session.SessionSystem.NextRun.Value);
                        }
                        

                        if (session.SessionCommunicationParameters.NeedResetCommunication)
                        {
                            ResetCommunication(session);
                        }
                        session.NewSession();
                    }

                }

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
