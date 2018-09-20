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
    [Serializable]
    public class CommunicationBlock
    {

        private bool _running = false;
        private List<byte[]> _reports = new List<byte[]>();
        private System.Threading.Timer _timer;

        private IReportPattern _messageContinuePattern = new ReportPattern(0x3c, 4);
        private IDevice _device;
        private CancellationToken _cancelToken;
        private ILogger Logger = ApplicationLogging.CreateLogger<CommunicationBlock>();
        private int delay = 100;
        public bool Erorrs { get; set; }
        protected BlockingCollection<AstmStart> ResponsesRecieved { get; set; } = new BlockingCollection<AstmStart>();

        public SerializerSession Session { get; set; } = new SerializerSession();

        public int TimeoutSeconds { get; set; }

        public AstmStart Request { get; set; }

        public BlockingCollection<IReportPattern> ExpectedResponses { get; set; } = new BlockingCollection<IReportPattern>();

        public bool LogDataRecieved { get; set; } = true;

        public List<List<byte>> GetRequestBytes()
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

        public async Task StartCommunication(IDevice device, SerializerSession session, CancellationToken cancelToken)
        {
            int periode = 0;
            _cancelToken = cancelToken;
            Session = session;
            _device = device;

            if (TimeoutSeconds == 0)
            {
                TimeoutSeconds = session.MinimedConfiguration().TimeoutSeconds;
                //Logger.LogInformation($"Timeout in Seconds: {TimeoutSeconds}");
            }


            if (_device.IsConnected)
            {
                var lists = this.GetRequestBytes();

                StartTimer();
                _device.DoneReceived = false;
                Log(lists);
                _device.SendBytes(lists);

                //foreach (var item in lists)
                //{
                //    var newarr = item.ToArray<byte>();
                //    Log(newarr);
                //    _device.SendBytes(newarr);
                   
                //}

                if (this.ExpectedResponses.Count > 0)
                {
                    //wait for all responses or timeout for whole block.
                    while (this._running && _device.IsConnected)
                    {

                        periode += delay;
                        Task.Delay(delay).Wait();
                        if ((TimeoutSeconds * 1000) <= periode)
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
            lock (this._reports)
            {


                bool _waitForMoreReports = false;
                if (bytes == null)
                {
                    StopTimer();
                    return;
                }
                if (this._messageContinuePattern.Evaluate(bytes))
                {
                    _waitForMoreReports = true;
                }
                else
                {
                    _waitForMoreReports = false;
                }

                _reports.Add(bytes);
                if (LogDataRecieved)
                {
                    Log(bytes);
                }


                if (_waitForMoreReports == false)
                {
                    var response = this.ExpectedResponses.Take();
                    var firstReport = this._reports.First();//[0];

                    if (response.Evaluate(firstReport))
                    {
                        try
                        {
                            var temp = this._reports.JoinToArray();
                            Serializer serializer = new Serializer(Session);
                            var resp = serializer.Deserialize<AstmStart>(temp);
                            ResponsesRecieved.Add(resp);

                        }
                        catch (Exception x)
                        {
                            CommunicationError(x.Message);
                        }

                        _reports = new List<byte[]>();
                    }
                    else
                    {
                        Logger.LogError("Unexpected message received: " + BitConverter.ToString(firstReport));
                    }

                    if (this.ExpectedResponses.Count == 0)
                    {
                        StopTimer();
                        _device.DoneReceived = true;
                    }
                }

            }
        }

        private void Log(List<List<byte>> bytes)
        {
            bytes.ForEach(e => Log(e.ToArray()));
        }

            private void Log(byte[] bytes)
        {
            string bytesStr = BitConverter.ToString(bytes);
            Logger.LogTrace(bytesStr);
        }

        private void StartTimer()
        {
            //ResponsesRecieved = new BlockingCollection<AstmStart>();
            _reports = new List<byte[]>();
            _running = true;
            _device.DataReceived += _device_DataReceived;
            var autoEvent = new AutoResetEvent(false);
            _timer = new Timer(TimeIsUp, autoEvent, TimeoutSeconds * 1000, TimeoutSeconds * 1000);
        }

        //private void _device_MultiDataReceived(object sender, List<byte[]> data)
        //{
        //    lock (this.ExpectedResponses)
        //    {
        //        var response = this.ExpectedResponses.Take();
        //        var firstReport = data.First();//[0];
        //        if (response.Evaluate(firstReport))
        //        {
        //            try
        //            {
        //                foreach (var item in data)
        //                {
        //                    if (LogDataRecieved)
        //                    {
        //                        Log(item);
        //                    }
        //                }
        //                Serializer serializer = new Serializer(Session);

        //                var temp = data.JoinToArray();
        //                var resp = serializer.Deserialize<AstmStart>(temp);
        //                ResponsesRecieved.Add(resp);
        //            }
        //            catch (Exception x)
        //            {
        //                CommunicationError(x.Message);
        //            }

        //            _reports = new List<byte[]>();
        //        }
        //        else
        //        {
        //            Logger.LogError("Unexpected message received: " + BitConverter.ToString(firstReport));

        //            //CommunicationError("Unexpected message received.");
        //        }

        //        if (this.ExpectedResponses.Count == 0)
        //        {
        //            StopTimer();
        //        }
        //    }
        //}

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
            _reports = new List<byte[]>();
            _device.DataReceived -= _device_DataReceived;
            _running = false;


        }

        private void CommunicationError(string error)
        {
            this.ExpectedResponses = new BlockingCollection<IReportPattern>();
            this.Erorrs = true;

            StopTimer();
            Logger.LogError(error);
        }

    }
}
