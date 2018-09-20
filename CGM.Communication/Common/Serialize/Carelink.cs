using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CGM.Communication.Common.Serialize
{
    public class ImportCarelink
    {
        private string _path;
        private string[] lines;
        private int to;
        private int _from = 11;
        public List<ImportLine> Lines { get; set; } = new List<ImportLine>();
        public int MaxLines { get; set; }
        private Dictionary<int, string> lineErrors = new Dictionary<int, string>();
        public ImportCarelink(string path, int maxLines, int from)
        {
            MaxLines = maxLines;
            _path = path;
            _from = from;
            lines = System.IO.File.ReadAllLines(_path);

            to = lines.Length;
            if (MaxLines != 0 && to > MaxLines)
            {
                to = MaxLines + _from;
            }

            CheckLines();
            if (lineErrors.Count > 0)
            {
                throw new Exception("LineErrors");
            }
            ImportLines();
        }

        public ImportCarelink(string path, int maxLines) : this(path, maxLines, 11)
        {

        }

        public ImportCarelink(string path) : this(path, 0)
        {

        }

        private void CheckLines()
        {
            for (int i = 12; i < to; i++)
            {
                var firstcomma = lines[i].IndexOf(',');
                if (firstcomma > 0)
                {
                    var str = lines[i].Substring(0, firstcomma);
                    if (!int.TryParse(str, out int result))
                    {
                        lineErrors.Add(i, lines[i]);
                    }
                }

            }
        }

        private void ImportLines()
        {
            //string path = @"carelink\files\CareLink-Export-1496858979253.csv";

            //bool foundfirst = false;

            for (int i = _from; i < to; i++)
            {
                //if (foundfirst)
                //{
                if (lines[i].Length > 0)
                {
                    Lines.Add(new ImportLine(lines[i]));
                    //Debug.WriteLine(i);
                }

                //}
                //else
                //{
                //    if (lines[i].StartsWith("1,") || lines[i].StartsWith("\"1,"))
                //    {
                //        //firste line
                //        foundfirst = true;
                //    }
                //}


            }
        }
    }
    public class ImportLine
    {
        public string Line { get; set; }
        public string[] Values { get; set; }
        public DateTime DateTime { get; set; }
        public int Number { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>();

        public ImportLine(string line)
        {
            Line = line;
            SplitLine();
        }

        private void SplitLine()
        {


            Values = GetValues(Line);

            Number = int.Parse(Values[0]);
            DateTime = DateTime.Parse(Values[3]);
            Type = Values[33];
            Data = Values[34].Replace("\"", "").Replace(" ", "");



            //var splitByEqual = new[] { '=' };

            //KeyValues = Regex.Split(Data, @",(?=\w+=)")
            //    .Select(token => token.Split(splitByEqual, 2))
            //    .ToDictionary(pair => pair.Length == 1 ? "" : pair.First(),
            //                  pair => pair.Last());



        }


        private string[] GetValues(string line)
        {

            string delimiter = ",";
            string textQualifier = "\"";
            string reString = string.Format(@"{0}(?=(?:[^{1}]*{1}[^{1}]*{1})*(?![^{1}]*{1}))", Regex.Escape(delimiter), Regex.Escape(textQualifier));
            Regex re = new Regex(reString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            return re.Split(line);

        }

        public override string ToString()
        {
            return $"{this.DateTime} - {this.Type}";
        }
    }
}
