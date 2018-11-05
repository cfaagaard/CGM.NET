using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    public class LogFileHistoryAdapter
    {
        private readonly string logPath;
        public List<LogFileChuncker> logFileChunckers = new List<LogFileChuncker>();

        //public List<BasePumpEvent> PumpEvents { get; set; } = new List<BasePumpEvent>();
        //public List<BasePumpEvent> SensorEvents { get; set; } = new List<BasePumpEvent>();
        public int MaxNumber { get; set; } = 0;


        public List<SerializerSession> CollectedSessions { get; set; } = new List<SerializerSession>();

        public LogFileHistoryAdapter(string logPath)
        {
            this.logPath = logPath;

        }

        public void ExtractHistory(DateTime from, DateTime to, bool allHistory)
        {
            string logfileFormat = "{0}_log.txt";

            int countDaysDiff = to.Subtract(from).Days + 1;

            for (int i = 0; i < countDaysDiff; i++)
            {
                DateTime fromDate = from.AddDays(i);
                string fullpath = Path.Combine(this.logPath, string.Format(logfileFormat, fromDate.ToString("ddMMyyyy")));
                if (File.Exists(fullpath))
                {
                    var chuncker = new LogFileChuncker(fullpath);
                    chuncker.MaxNumber = MaxNumber;
                    logFileChunckers.Add(chuncker);

                    chuncker.GetAllMessages();

                    if (allHistory)
                    {
                        chuncker.GetAllSessions();
                    }
                    else
                    {
                        var first = chuncker.chuncks.First(e => e.session.PumpDataHistory.MultiPacketHandlers.Count > 0);
                        var last = chuncker.chuncks.Last(e => e.session.PumpDataHistory.MultiPacketHandlers.Count > 0);

                        first.session.PumpDataHistory.ExtractHistoryEvents();
                        last.session.PumpDataHistory.ExtractHistoryEvents();

                        var maxPumpEventDate = first.session.PumpDataHistory.PumpEvents.Max(e => e.EventDate.DateTime.Value);
                        var maxSensorEventDate = first.session.PumpDataHistory.SensorEvents.Max(e => e.EventDate.DateTime.Value);

                        var pumpevents = last.session.PumpDataHistory.PumpEvents.Where(e => e.EventDate.DateTime.Value > maxPumpEventDate).ToList();
                        var sensorvents = last.session.PumpDataHistory.SensorEvents.Where(e => e.EventDate.DateTime.Value > maxSensorEventDate).ToList();

                        first.session.PumpDataHistory.PumpEvents.AddRange(pumpevents);
                        first.session.PumpDataHistory.SensorEvents.AddRange(sensorvents);

                        var liststatus=chuncker.chuncks.Select(e => e.session.Status);

                        first.session.Status = new List<MiniMed.Responses.PumpStatusMessage>();

                        foreach (var statusses in liststatus)
                        {
                            foreach (var status in statusses)
                            {
                                first.session.Status.Add(status);
                            }
                        }

                        CollectedSessions.Add(first.session);
                        //chuncker.GetLastSession();
                        //if (chuncker.Session != null)
                        //{
                        //    DateTime toDate = fromDate.AddHours(24);

                        //    PumpEvents.AddRange(chuncker.Session.PumpDataHistory.PumpEvents.Where(e => e.EventDate.DateTime.Value >= fromDate && e.EventDate.DateTime.Value < toDate));
                        //    SensorEvents.AddRange(chuncker.Session.PumpDataHistory.SensorEvents.Where(e => e.EventDate.DateTime.Value >= fromDate && e.EventDate.DateTime.Value < toDate));
                        //}
                    }
                }

            }

        }

        //public void Save(string filePath)
        //{
        //    List<BasePumpEvent> all = new List<BasePumpEvent>(PumpEvents);
        //    all.AddRange(SensorEvents);

        //    List<EventExporter> eventExporters = new List<EventExporter>();

        //    //var events = all.OrderBy(e => e.EventDate);

        //    var events = all.Select(e => new EventExporter()
        //    {
        //        EventDate = e.EventDate.DateTimeString,
        //        EventType = e.EventType.ToString(),
        //        EventToString = e.ToString()
        //    });


        //    string content = CsvExporter.GenerateReport<EventExporter>(events.ToList());

        //    System.IO.File.AppendAllText(filePath, content);


        //}
    }


    public class EventExporter
    {
        public string EventDate { get; set; }
        public string EventType { get; set; }
        public string EventToString { get; set; }
    }

    public class LogFileChuncker
    {
        private readonly string filepath;
        public List<LogChunck> chuncks = new List<LogChunck>();
        public int MaxNumber { get; set; }

        public SerializerSession Session { get; set; }

        public LogFileChuncker(string filepath)
        {
            this.filepath = filepath;
            ChunckFile();
        }

        public void GetAllMessages()
        {
            foreach (var item in chuncks)
            {
                item.GetMessages();
            }
        }

        public void GetAllSessions()
        {

            int toNumber = chuncks.Count;
            if (MaxNumber != 0)
            {
                toNumber = MaxNumber;
            }

            for (int i = 0; i < toNumber; i++)
            {
                chuncks[i].session.PumpDataHistory.ExtractHistoryEvents();
            }
            //foreach (var item in chuncks)
            //{
            //    item.ExtractHistory();
            //}
        }


        public void GetLastSession()
        {
            chuncks.Reverse();


            foreach (var item in chuncks)
            {
                item.session.PumpDataHistory.ExtractHistoryEvents();
                if (item.session.PumpDataHistory.PumpEvents.Count > 0 && item.session.PumpDataHistory.SensorEvents.Count > 0)
                {
                    Session = item.session;
                    break;
                }
            }
        }

        private void ChunckFile()
        {
            if (System.IO.File.Exists(filepath))
            {
                var lines = File.ReadAllLines(filepath);
                LogChunck chunck = new LogChunck();
                for (int j = 0; j < lines.Count(); j++)
                {
                    if (lines[j].Contains("Connected to internet"))
                    {
                        //start
                        chunck = new LogChunck();
                        chuncks.Add(chunck);
                    }
                    chunck.Lines.Add(lines[j]);
                }



            }
        }
    }

    public class LogChunck
    {
        public List<string> Lines { get; set; } = new List<string>();
        ByteMessageCollection Messages;
        public SerializerSession session;
        public void GetMessages()
        {

            session = new SerializerSession();
            Messages = new ByteMessageCollection(session);
            int nr = 1;
            var parameter = Lines.FirstOrDefault(e => e.Contains("; RadioChannel:"));

            if (parameter != null)
            {
                int start = parameter.IndexOf(';') + 2;
                parameter = parameter.Substring(start).Trim();
                session.SetParametersByString(parameter);

                foreach (var item in Lines)
                {
                    var split = item.Split(';');
                    if (split.Length == 2)
                    {
                        string bytestr = split[1].Trim();
                        if (bytestr.StartsWith("00-"))
                        {
                            Messages.Add(bytestr.GetBytes(), nr, split[0]);
                            nr += 1;
                        }
                        else
                        {

                        }

                    }

                }

            }


        }

        public override string ToString()
        {
            if (session != null && session.PumpTime.PumpDateTime.HasValue)
            {
                return session.PumpTime.PumpDateTime.Value.ToString();
            }
            return "";
        }
    }
}
