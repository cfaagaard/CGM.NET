using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CGM.Communication.Extensions;
using System.Text.RegularExpressions;

namespace CGM.Communication.MiniMed.Responses
{
    public class GlucoseHiLo
    {
        public string glucoserangelo { get; set; }
        public string glucoserangehi { get; set; }

        public GlucoseHiLo(string value)
        {
            glucoserangelo = value.Substring(0, 2);
            glucoserangehi = value.Substring(2, 3);
        }
    }
    public class HighLowTargetRanges
    {
        public string personalhypoglycemiclimit { get; set; }
        public string personaloveralllow { get; set; }
        public string personalprefoodlow { get; set; }
        public string personalpostfoodlow { get; set; }
        public string personaloverallhigh { get; set; }
        public string personalprefoodhigh { get; set; }
        public string personalpostfoodhigh { get; set; }
        public string personalhyperglycemiclimit { get; set; }
        public string fastinglow { get; set; }
        public string fastinghigh { get; set; }

        public HighLowTargetRanges(string value)
        {
            //
            //"070 070 070 070 180 130 180 250 070 130"
            var values = SplitToLines(value, 3);
            if (values.Count==10)
            {
                personalhypoglycemiclimit = values[0];
                personaloveralllow = values[1];
                personalprefoodlow = values[2];
                personalpostfoodlow = values[3];
                personaloverallhigh = values[4];
                personalprefoodhigh = values[5];
                personalpostfoodhigh = values[6];
                personalhyperglycemiclimit = values[7];
                fastinglow = values[8];
                fastinghigh = values[9];
            }


        }
        private static List<string> SplitToLines(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }
    }

    public class DeviceVersion
    {
        public string DigitalEngineVersion { get; set; }
        public string AnalogEngineVersion { get; set; }
        public string GameBoardVersion { get; set; }

        public string Name { get; set; }

        public string ModelNumber { get; set; }

        public string Manufacturer { get; set; }

        public string RFID { get; set; }

        public string SerialNum{ get; set; }
        public string SerialNumSmall { get; set; }
        public string SkuIdentifier { get; set; }
    }
    public class BayerStickInfoReader
    {
        private string _value;
        public DeviceVersion DeviceVersion { get; set; }
        public string AccessPassword { get; set; }
        public string MeterLanguage { get; set; }
        public string TestReminderInterval { get; set; }

        //public HighLowTargetRanges Ranges { get; set; }

        //public GlucoseHiLo GlucoseHiLo { get; set; }

        public Byte[] HMACbyte { get; set; }
        public BayerStickInfoReader(string value)
        {
            _value = value;
            ReadValue();


        }

        private void ReadValue()
        {
            char fielddelimiter = '|';

            var msgsplit = _value.Split(fielddelimiter);

            char repeatdelimiter = msgsplit[1][0];
            char componentdelimeter = msgsplit[1][1];
            char escapedelimter = msgsplit[1][2];
            

            AccessPassword = msgsplit[3];
            //"Bayer6210^01.12\\01.04\\25.25\\01.00.A^6213-1001687^0000-^BG1001687B"
            var devicenumber = msgsplit[4];
            //"A=0^C=02^G=da,en\\ar\\zh\\hr\\cs\\da\\nl\\fi\\fr\\de\\el\\he\\hu\\it\\ja\\no\\pl\\pt\\ru\\sk\\sl\\es\\sv\\tr^I=0200^R=0^S=09^U=1^V=20600^X=070070070070180130180250070130^Y=360126099054120054252099^Z=1"
            string deviceinfo = msgsplit[5];

            var deviceInfoSplit = deviceinfo.Split(componentdelimeter);

            MeterLanguage = deviceInfoSplit.FirstOrDefault(e => e.StartsWith("G=")).Replace("G=","");
            TestReminderInterval = deviceInfoSplit.FirstOrDefault(e => e.StartsWith("I=")).Replace("I=", "");

            this.DeviceVersion = new DeviceVersion();

            //"Bayer6210^01.12\\01.04\\25.25\\01.00.A^6213-1001687^0000-^BG1001687B"
            var devicenumberSplit = devicenumber.Split(componentdelimeter);

            var versionSplit = devicenumberSplit[1].Split(repeatdelimiter);

            this.DeviceVersion.DigitalEngineVersion = versionSplit[0];
            this.DeviceVersion.AnalogEngineVersion = versionSplit[1];
            this.DeviceVersion.GameBoardVersion = versionSplit[2];


            //var modelsplit = devicenumberSplit[0].Split(escapedelimter);


            this.DeviceVersion.Name = devicenumberSplit[0];
            this.DeviceVersion.ModelNumber = new String(this.DeviceVersion.Name.Where(Char.IsDigit).ToArray());
            this.DeviceVersion.Manufacturer = this.DeviceVersion.Name.Replace(this.DeviceVersion.ModelNumber, "");

            var modelsplit2 = devicenumberSplit[3].Split(escapedelimter);
            this.DeviceVersion.SerialNum = devicenumberSplit[2];
            this.DeviceVersion.SkuIdentifier = devicenumberSplit[3];
            this.DeviceVersion.RFID = devicenumberSplit[4];
            this.DeviceVersion.SerialNumSmall = Regex.Replace(this.DeviceVersion.RFID, "[^0-9.]", "");

            string HmacPadding = "A4BD6CED9A42602564F413123";


            string total = this.DeviceVersion.SerialNumSmall + HmacPadding;
            byte[] totalBytes = Encoding.UTF8.GetBytes(total);
            byte[] toBytes = totalBytes.Sha256Digest();
            Array.Reverse(toBytes);

            this.HMACbyte = toBytes;

            //is not used.
            //this.Ranges = new HighLowTargetRanges(deviceInfoSplit.FirstOrDefault(e => e.StartsWith("X=")).Replace("X=", ""));
            //this.GlucoseHiLo = new GlucoseHiLo(deviceInfoSplit.FirstOrDefault(e => e.StartsWith("V=")).Replace("V=", ""));

        }
    }
}
