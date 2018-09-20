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
        private List<LogFileChuncker> logFileChunckers = new List<LogFileChuncker>();

        public List<BasePumpEvent> PumpEvents { get; set; } = new List<BasePumpEvent>();
        public List<BasePumpEvent> SensorEvents { get; set; } = new List<BasePumpEvent>();

        public LogFileHistoryAdapter(string logPath)
        {
            this.logPath = logPath;

        }

        public void ExtractHistory(DateTime from, DateTime to,bool allHistory)
        {
            string logfileFormat = "{0}_log.txt";

            int countDaysDiff = to.Subtract(from).Days + 1;

            for (int i = 0; i < countDaysDiff; i++)
            {
                DateTime fromDate = from.AddDays(i);
                string fullpath = Path.Combine(this.logPath, string.Format(logfileFormat, fromDate.ToString("ddMMyyyy")));
                var chuncker = new LogFileChuncker(fullpath);
                logFileChunckers.Add(chuncker);
                if (allHistory)
                {
                    chuncker.GetAllSessions();
                }
                else
                {
                    chuncker.GetLastSession();
                    if (chuncker.Session != null)
                    {
                        DateTime toDate = fromDate.AddHours(24);

                        PumpEvents.AddRange(chuncker.Session.PumpDataHistory.PumpEvents.Where(e => e.EventDate.DateTime.Value >= fromDate && e.EventDate.DateTime.Value < toDate));
                        SensorEvents.AddRange(chuncker.Session.PumpDataHistory.SensorEvents.Where(e => e.EventDate.DateTime.Value >= fromDate && e.EventDate.DateTime.Value < toDate));
                    }
                }







            }

        }


        public void Save(string filePath)
        {
            List<BasePumpEvent> all = new List<BasePumpEvent>(PumpEvents);
            all.AddRange(SensorEvents);

            List<EventExporter> eventExporters = new List<EventExporter>();

            //var events = all.OrderBy(e => e.EventDate);
            
            var events = all.Select(e => new EventExporter()
            {
                EventDate = e.EventDate.DateTimeString,
                EventType = e.EventType.ToString(),
                EventToString=e.ToString()
            });


            string content = CsvExporter.GenerateReport<EventExporter>(events.ToList());

            System.IO.File.AppendAllText(filePath, content);


        }


    }


    public class EventExporter
    {
        public string EventDate { get; set; }
        public string EventType { get; set; }
        public string EventToString { get; set; }
    }

    class LogFileChuncker
    {
        private readonly string filepath;
        private List<LogChunck> chuncks = new List<LogChunck>();

        public SerializerSession Session { get; set; }

        public LogFileChuncker(string filepath)
        {
            this.filepath = filepath;
            ChunckFile();
        }

        public void GetAllSessions()
        {
            foreach (var item in chuncks)
            {
                item.ExtractHistory();
            }
        }


        public void GetLastSession()
        {
            chuncks.Reverse();


            foreach (var item in chuncks)
            {

                item.ExtractHistory();
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

    class LogChunck
    {
        public List<string> Lines { get; set; } = new List<string>();
        ByteMessageCollection Messages;
        internal SerializerSession session;
        public void ExtractHistory()
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
                session.PumpDataHistory.ExtractHistoryEvents();
            }


        }

        public override string ToString()
        {
            if (session!=null && session.PumpTime.PumpDateTime.HasValue)
            {
                return session.PumpTime.PumpDateTime.Value.ToString();
            }
            return "";
        }
    }
}
