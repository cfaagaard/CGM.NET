using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.Patterns;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGM.Communication.MiniMed
{
    public class CommunicationBlock
    {

        private bool _running = false;
        private List<byte[]> _reports = new List<byte[]>();
        private System.Threading.Timer _timer;
        private bool _waitForMoreReports = false;
        private IReportPattern _messageContinuePattern = new ReportPattern(0x3c, 4);
        private IDevice _device;
        private CancellationToken _cancelToken;
        private ILogger Logger = ApplicationLogging.CreateLogger<CommunicationBlock>();
        private int delay = 250;
        //private int maxdelay = 7000;
        //private int recievedBytesLength = 0;
        //public int MaxRecievedBytesLength { get; set; } = -1;
        public int maxdelay { get; set; } = 7000;

        protected BlockingCollection<AstmStart> ResponsesRecieved { get; set; } = new BlockingCollection<AstmStart>();

        public SerializerSession Session { get; set; } = new SerializerSession();

        public int TimeoutSeconds { get; set; } = 7;
        public AstmStart Request { get; set; }
        public BlockingCollection<IReportPattern> ExpectedResponses { get; set; } = new BlockingCollection<IReportPattern>();

    

        private List<List<byte>> GetRequestBytes()
        {
            Serializer serializer = new Serializer(Session);
            byte[] bytes = serializer.Serialize<AstmStart>(this.Request);


            List<byte> bytelist = new List<byte>(bytes);
            List<byte> start = bytelist.GetRange(0, 5);
            List<byte> rest = bytelist.GetRange(5, bytelist.Count - 5);
            var lists = rest.SplitList(60).ToList();

            for (int i = 0; i < lists.Count; i++)
            {
                var item = lists[i];
                item.InsertRange(0, start);
                if (i != lists.Count - 1)
                {
                    item[4] = 0x3C;
                }
                else
                {
                    item[4] = (byte)(item.Count - 5);
                }

            }
            return lists;
        }

        public async Task StartCommunication(IDevice device,SerializerSession session,CancellationToken cancelToken)
        {
            int periode = 0;
            _cancelToken = cancelToken;
            Session = session;
            _device = device;

            if (_device.IsConnected)
            {
                var lists = this.GetRequestBytes();

                StartTimer();
                foreach (var item in lists)
                {
                    Log(item.ToArray());
                    await _device.SendBytes(item.ToArray());
                }

                if (this.ExpectedResponses.Count > 0)
                {
                    //wait for all responses or timeout for whole block.
                    while (this._running && _device.IsConnected)
                    {
                        periode += delay;
                        Task.Delay(delay).Wait();
                        if (maxdelay <= periode)
                        {
                            CommunicationError("Error: timeout");
                            return;
                        }
                    }
                }
                StopTimer();
            }
        }


        public virtual void _device_DataReceived(object sender, byte[] bytes)
        {
            if (bytes == null)
            {
                StopTimer();
                return;
            }
            //this.recievedBytesLength += bytes.Length;
            if (this._messageContinuePattern.Evaluate(bytes))
            {
                _waitForMoreReports = true;
            }
            else
            {
                _waitForMoreReports = false;
            }

            _reports.Add(bytes);

            Log(bytes);

            if (_waitForMoreReports == false)
            {
                //if (this._communicationBlock != null)
                //{
                    var response = this.ExpectedResponses.Take();
                //if (MaxRecievedBytesLength != -1 && MaxRecievedBytesLength > recievedBytesLength)
                //{
                //    this.ExpectedResponses.Add(response);
                //}

                    if (response.Evaluate(this._reports[0]))
                    {
                        try
                        {
                            Serializer serializer = new Serializer(Session);

                            var temp = this._reports.JoinToArray();
                            var resp = serializer.Deserialize<AstmStart>(temp);
                            //this.ResponsesRecieved.Add(resp);
                    }
                        catch (Exception x)
                        {
                            CommunicationError(x.Message);
                        }

                        _reports = new List<byte[]>();
                    }
                    else
                    {

                        CommunicationError("Error. Unhandled");
                    }

                    if (this.ExpectedResponses.Count == 0)
                    {
                        StopTimer();
                    }
                //}
                //else
                //{
                //    try
                //    {
                //        Serializer serializer = new Serializer(Session);

                //        var temp = this._reports.JoinToArray();
                //        var resp = serializer.Deserialize<AstmStart>(temp);
                //        this.ResponsesRecieved.Add(resp);
                //    }
                //    catch (Exception x)
                //    {
                //        CommunicationError(x.Message);
                //    }

                //    _reports = new List<byte[]>();
                //}

            }

        }

        private void Log(byte[] bytes)
        {
            string bytesStr = BitConverter.ToString(bytes);
            Logger.LogTrace(bytesStr);
        }

        private void StartTimer()
        {
            ResponsesRecieved = new BlockingCollection<AstmStart>();
            _reports = new List<byte[]>();
            _running = true;
            _device.DataReceived += _device_DataReceived;
            var autoEvent = new AutoResetEvent(false);

            //PeriodicTaskFactory.Start(() =>
            //{
            //    if (_running)
            //    {
            //        CommunicationError($"Error: timeout - ({this.GetType().Name})");
            //    }

            //}, _communicationBlock.TimeoutSeconds * 1000, _communicationBlock.TimeoutSeconds * 1000,5,1,true, _cancelToken);

            _timer = new Timer(TimeIsUp, autoEvent, TimeoutSeconds * 1000, TimeoutSeconds * 1000);

        }
        private void TimeIsUp(Object stateInfo)
        {
            string timeout = $"Error: timeout - ({this.GetType().Name})";
            CommunicationError(timeout);

        }

        private void StopTimer()
        {

            if (_timer != null)
            {
                lock (_timer)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    // _timer.Dispose();
                }
            }

            _device.DataReceived -= _device_DataReceived;
            _running = false;
        }


        private void CommunicationError(string error)
        {
            StopTimer();
            Logger.LogError(error);



        }
        //p

    }
}
