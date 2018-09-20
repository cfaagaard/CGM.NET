using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.Common
{
    public enum TaskStatusEnum
    {
        Starting,
        Stopping,
        Running,
        Stopped,
    }
    public class CgmTask : ICgmTask
    {
        private readonly IDevice device;
        private readonly IStateRepository stateRepository;
        private readonly ISessionBehaviors sessionTasks;
        protected ILogger Logger = ApplicationLogging.CreateLogger<CgmTask>();
        protected SerializerSession session;
        protected CancellationToken _token;
        protected CancellationTokenSource _tokenSource;
        protected int _delayInSeconds = 150;
        protected TimeSpan Delay;
        protected int Intervalseconds;
        protected System.Threading.Timer timer;
        private TaskStatusEnum _status = TaskStatusEnum.Stopped;

        public event EventHandler StatusChanged;

        public IDevice Device { get { return device; } }

        public TaskStatusEnum Status
        {
            get => _status; set
            {
                _status = value;
                StatusChanged?.Invoke(this, null);
            }
        }

        public CgmTask(IDevice device, IStateRepository stateRepository, ISessionBehaviors sessionTasks)
        {
            this.device = device;
            this.stateRepository = stateRepository;
            this.sessionTasks = sessionTasks;
            this.device.DeviceAdded += Device_DeviceAdded;
            this.device.DeviceRemoved += Device_DeviceRemoved;

            if (device.IsConnected)
            {
                Start();
            }
        }

        private void Device_DeviceRemoved(object sender, EventArgs e)
        {
            Stop();
        }

        private void Device_DeviceAdded(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            Status = TaskStatusEnum.Starting;
            Delay = TimeSpan.FromSeconds(_delayInSeconds);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            session = new SerializerSession();
            
            stateRepository.GetOrSetSessionAndSettings(session);

            Intervalseconds = session.MinimedConfiguration().IntervalSeconds;

            SetUpTimer(DateTime.Now.AddSeconds(2));

            Status = TaskStatusEnum.Running;
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

        protected virtual async Task GetData()
        {

            try
            {
                ISessionFactory sessionGetter = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISessionFactory>();
                session = await sessionGetter.GetPumpSessionAsync(session, _token);
            }
            catch (Exception)
            {

                throw;
            }

            if (!_token.IsCancellationRequested)
            {
                if (session != null)
                {
                    await GotSession(session);

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

        protected async virtual Task GotSession(SerializerSession session)
        {
            if (Log.StaticLogs.Logs!=null)
            {
                session.Logs = StaticLogs.Logs;
            }

            if (sessionTasks != null && sessionTasks.SessionBehaviors.Count > 0)
            {
                foreach (var item in sessionTasks.SessionBehaviors)
                {
                    try
                    {
                        await item.ExecuteTask(session, _token).TimeoutAfter(15000);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error in executing sessionTask: {ex.Message}");
                    }
                }
            }
        }

        protected virtual void ResetCommunication(SerializerSession session)
        {

        }

        public void Stop()
        {
            Status = TaskStatusEnum.Stopping;
            if (this.timer != null)
            {
                timer.Dispose();
            }
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }

            Status = TaskStatusEnum.Stopped;

        }
    }
}
