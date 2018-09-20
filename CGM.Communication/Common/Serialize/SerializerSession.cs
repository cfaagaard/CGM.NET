using System;
using CGM.Communication.MiniMed;
using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Requests;
using CGM.Communication.MiniMed.Responses;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Extensions;
using System.Threading.Tasks;
using System.Linq;
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using CGM.Communication.Common.Serialize.Log;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CGM.Communication.Interfaces;
using Newtonsoft.Json;

namespace CGM.Communication.Common.Serialize
{
    [Serializable]
    public class SerializerSession
    {
        [NonSerialized]
        protected ILogger Logger = ApplicationLogging.CreateLogger<SerializerSession>();
        private char fieldDelimeter = ';';
        private char keyvalueDelimeter = ':';
        #region properties

        public SessionSystem SessionSystem { get; set; } = new SessionSystem();
        public SessionDevice SessionDevice { get; set; } = new SessionDevice();
        public SessionCommunicationParameters SessionCommunicationParameters { get; set; }
        public SessionVariables SessionVariables { get; set; } = new SessionVariables();

        public Dictionary<string,IConfiguration> Configurations { get; set; } = new Dictionary<string, IConfiguration>();

        public PumpSettings PumpSettings { get; set; } = new PumpSettings();
        public List<PumpStatusMessage> Status { get; set; } = new List<PumpStatusMessage>();
        public PumpTimeMessage PumpTime { get; set; } = new PumpTimeMessage();
        public PumpDataHistory PumpDataHistory { get; set; }
        public List<LogEntry> Logs { get; set; } = new List<LogEntry>();
        public int DataLoggerBattery { get; set; } = 100;
        public bool CanSaveSession
        {
            get
            {
                return !string.IsNullOrEmpty(this.SessionDevice.Device.SerialNumber);
            }
        }


        public bool GotConnectionToPump
        {
            get
            {
                return this.SessionCommunicationParameters.RadioChannel != 0x00 && this.PumpTime != null;
            }
        }

        #endregion

        public SerializerSession()
        {
            NewSession();
        }

        public void NewSession()
        {
            this.SessionCommunicationParameters = new SessionCommunicationParameters(this);
            this.SessionVariables = new SessionVariables();
            this.Status = new List<PumpStatusMessage>();
            this.PumpDataHistory = new PumpDataHistory(this);
            this.PumpSettings = new PumpSettings();
            this.SessionSystem.NewSession();
            this.Logs = new List<LogEntry>();


        }

        public T GetConfiguration<T>() where T : IConfiguration
        {
            string key = typeof(T).Name;
            if (this.Configurations.Keys.Contains(key))
            {
               return (T)this.Configurations.FirstOrDefault(e => e.Key == typeof(T).Name).Value;
            }
            else
            {
                
                T newConfig;
                //string configurationFile = $"{typeof(T).Name}.json";
                //if (File.Exists(configurationFile))
                //{
                //    newConfig=JsonConvert.DeserializeObject<T>(File.ReadAllText(configurationFile));
                //}
                //else
                //{
                newConfig = (T)Activator.CreateInstance(typeof(T));
                //}
                
                this.Configurations.Add(key, newConfig);
                var repository=CommonServiceLocator.ServiceLocator.Current.GetInstance<IStateRepository>();
                repository.SaveConfiguration(newConfig);
                return newConfig;
            }
        }

        public void AddStatus(PumpStatusMessage status)
        {
            this.Status.Add(status);
            //calculate best next run
            //LocalDateTime   { 09 / 05 / 2017 21.56.30}
            //PumpDateTime    { 09 / 05 / 2017 21.59.30}
            //SgvDate         { 09 / 05 / 2017 21.59.00}
            //next sgvdate    { 09 / 05 / 2017 22.04.00}
            //seconds: 20

            //optimal, in pumptime: 22:04:20
            //optimal, in localtime: 22:01:20
            int seconds = 60;
            int minutes = 5;
            if (status.SgvDateTime.DateTime.HasValue && this.PumpTime.PumpDateTime.HasValue)
            {
                var nextSgvDate = status.SgvDateTime.DateTime.Value.AddMinutes(minutes);
                var optimalInPumptime = nextSgvDate.AddSeconds(seconds);
                //difference between local and pumpe time
                TimeSpan PumpDiffTime = DateTime.Now.Subtract(this.PumpTime.PumpDateTime.Value);
                this.SessionSystem.OptimalNextReadInPumpTime = optimalInPumptime;
                this.SessionSystem.OptimalNextRead = optimalInPumptime.Add(PumpDiffTime);
            }
        }

        public string GetParametersAsString()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add(nameof(SessionCommunicationParameters.RadioChannel), BitConverter.ToString(new byte[] { this.SessionCommunicationParameters.RadioChannel }));
            parameters.Add(nameof(SessionCommunicationParameters.LinkMac), BitConverter.ToString(this.SessionCommunicationParameters.LinkMac));
            parameters.Add(nameof(SessionCommunicationParameters.PumpMac), BitConverter.ToString(this.SessionCommunicationParameters.PumpMac));
            parameters.Add(nameof(SessionCommunicationParameters.LinkKey), BitConverter.ToString(this.SessionCommunicationParameters.LinkKey));
            parameters.Add(nameof(SessionCommunicationParameters.EncryptKey), BitConverter.ToString(this.SessionCommunicationParameters.EncryptKey));
            parameters.Add(nameof(SessionDevice.Device.SerialNumber), this.SessionDevice.Device.SerialNumber);
            parameters.Add(nameof(SessionDevice.Device.ModelNumber), this.SessionDevice.Device.ModelNumber);
            parameters.Add(nameof(SessionDevice.Device.SerialNumberFull), this.SessionDevice.Device.SerialNumberFull);
            StringBuilder builder = new StringBuilder();
            foreach (var item in parameters)
            {
                builder.Append($"{item.Key}{keyvalueDelimeter}{item.Value}{fieldDelimeter}");
            }
            return builder.ToString();

        }

        public void SetParametersByString(string parametersString)
        {
            var param = parametersString.Split(fieldDelimeter);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            foreach (var item in param)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var spl = item.Split(keyvalueDelimeter);
                    parameters.Add(spl[0], spl[1]);
                }

            }

            this.SessionCommunicationParameters.RadioChannel = parameters[nameof(SessionCommunicationParameters.RadioChannel)].GetBytes()[0];

            this.SessionCommunicationParameters.LinkMac = parameters[nameof(SessionCommunicationParameters.LinkMac)].GetBytes();
            this.SessionCommunicationParameters.PumpMac = parameters[nameof(SessionCommunicationParameters.PumpMac)].GetBytes();
            this.SessionCommunicationParameters.LinkKey = parameters[nameof(SessionCommunicationParameters.LinkKey)].GetBytes();
            this.SessionDevice.Device.SerialNumber = parameters[nameof(SessionDevice.Device.SerialNumber)];
            this.SessionDevice.Device.ModelNumber = parameters[nameof(SessionDevice.Device.ModelNumber)];
            this.SessionDevice.Device.SerialNumberFull = parameters[nameof(SessionDevice.Device.SerialNumberFull)];
        }


        public void Save(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();



        }


        #region FactoryMethodes

        private AstmStart GetNewRequest(byte sessionNumber, byte commandType)
        {
            AstmStart astm = new AstmStart(new byte[] { 0x51, 0x03 });
            astm.Message2 = new MedtronicMessage2(sessionNumber, commandType);
            return astm;
        }

        private AstmStart GetNewRequest(AstmCommandType commandType)
        {
            return GetNewRequest((byte)SessionVariables.GetNextSessionNumber(), (byte)commandType);
        }

        public AstmStart GetOpenConnectionRequest()
        {
            AstmStart msg = GetNewRequest(AstmCommandType.OPEN_CONNECTION);
            ((MedtronicMessage2)msg.Message2).Message = new ConnectionRequest(SessionDevice.Device.HMACbyte);
            return msg;
        }

        public AstmStart GetReadInfoRequest()
        {
            return GetNewRequest(AstmCommandType.READ_INFO);
        }

        public AstmStart GetLinkKeyRequest()
        {
            return GetNewRequest(AstmCommandType.REQUEST_LINK_KEY);

        }

        public AstmStart GetChannelRequest(byte radioChannel)
        {
            AstmStart msg = GetNewRequest((byte)SessionVariables.GetNextSessionNumber(), (byte)AstmCommandType.SEND_MESSAGE);
            ((MedtronicMessage2)msg.Message2).Message = new RadioChannelRequest(radioChannel, SessionCommunicationParameters.LinkMac, SessionCommunicationParameters.PumpMac);
            return msg;
        }

        private AstmStart GetPumpEnvelope(byte prefix, AstmSendMessageType messageType, byte[] message)
        {
            AstmStart msg = GetNewRequest((byte)SessionVariables.GetNextSessionNumber(), (byte)AstmCommandType.SEND_MESSAGE);
            PumpEnvelope penv = new PumpEnvelope(this.SessionCommunicationParameters.PumpMac, (byte)SessionVariables.GetNextSequenceNumber());
            penv.Message = new PumpMessage(prefix, messageType, message);
            ((MedtronicMessage2)msg.Message2).Message = penv;
            return msg;
        }

        private AstmStart GetPumpEnvelope(AstmSendMessageType type, byte[] message)
        {
            return GetPumpEnvelope((byte)SessionVariables.GetCryptedSequenceNumber(), type, message);
        }

        private AstmStart GetPumpEnvelope(AstmSendMessageType type)
        {
            return GetPumpEnvelope((byte)SessionVariables.GetCryptedSequenceNumber(), type, null);
        }

        public AstmStart GetBeginEHSM()
        {
            return GetPumpEnvelope(0x80, AstmSendMessageType.HIGH_SPEED_MODE_COMMAND, new byte[] { 0x00 });
        }

        public AstmStart GetSetting(AstmSendMessageType type)
        {
            return GetPumpEnvelope(type);
        }

        public AstmStart GetSetting(AstmSendMessageType type, byte[] message)
        {
            return GetPumpEnvelope(type, message);
        }

        public AstmStart GetDeviceChar()
        {
            List<byte> message = new List<byte>(this.SessionCommunicationParameters.PumpMac);
            message.Add(2);
            return GetPumpEnvelope(AstmSendMessageType.DEVICE_CHARACTERISTICS_REQUEST, message.ToArray());
        }

        public AstmStart GetRemoteBolus()
        {
            List<byte> message = new List<byte>(this.SessionCommunicationParameters.PumpMac);
            message.Add(2);
            return GetPumpEnvelope(AstmSendMessageType.REMOTE_BOLUS_REQUEST, message.ToArray());
        }

        public AstmStart GetPumpBasalPattern(byte patternNumber)
        {
            return GetPumpEnvelope(AstmSendMessageType.READ_BASAL_PATTERN_REQUEST, new byte[] { patternNumber });
        }

        public AstmStart GetReadHistoryInfo(HistoryDataTypeEnum historyDataType)
        {
            try
            {
                if (this.PumpTime != null && this.PumpTime.OffSet.Length == 4)
                {
                    int lastRtc = GetLastRtc(historyDataType);

                    Logger.LogInformation($"Getting history for {historyDataType.ToString()} from {this.PumpTime.GetDateTime(BitConverter.GetBytes(lastRtc)).ToString()}");
                    AstmStart msg = GetPumpEnvelope(AstmSendMessageType.READ_HISTORY_INFO_REQUEST);
                    ((PumpEnvelope)((MedtronicMessage2)msg.Message2).Message).Message.Message = new ReadHistoryInfoRequest(lastRtc, historyDataType);
                    return msg;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return null;
        }

        private int GetLastRtc(HistoryDataTypeEnum historyDataType)
        {
            //var lastRead = this.Settings.LastRead.FirstOrDefault(e => e.DataType == (int)historyDataType);
            var configuration = this.MinimedConfiguration();
            var lastRead = configuration.LastRead.FirstOrDefault(e => e.DataType == (int)historyDataType);

            int lastRtc = 0;
            if (lastRead == null)
            {
                lastRtc = DateTime.Now.AddDays(-1 * configuration.HistoryDaysBack).GetRtcBytes(this.PumpTime.OffSet).GetInt32(0);
            }
            else
            {
                lastRtc = lastRead.LastRtc;
            }
            return lastRtc;
        }

        public AstmStart GetReadHistory(HistoryDataTypeEnum historyDataType, int expectedSize)
        {
            if (this.PumpTime != null && this.PumpTime.OffSet.Length == 4)
            {
                int lastRtc = GetLastRtc(historyDataType);
                AstmStart msg = GetPumpEnvelope(AstmSendMessageType.READ_HISTORY_REQUEST);
                ((PumpEnvelope)((MedtronicMessage2)msg.Message2).Message).Message.Message = new ReadHistoryRequest(lastRtc, historyDataType);
                return msg;
            }
            return null;
        }

        public AstmStart GetMultiPacket(byte[] bytes)
        {
            AstmStart msg = GetPumpEnvelope(AstmSendMessageType.ACK_MULTIPACKET_COMMAND);
            PumpGeneral request = new PumpGeneral();
            request.Unknown1 = bytes;//;

            ((PumpEnvelope)((MedtronicMessage2)msg.Message2).Message).Message.Message = request;
            return msg;
        }

        public AstmStart GetMissingSegments(UInt16 startPacket, UInt16 packetCount)
        {
            AstmStart msg = GetPumpEnvelope(AstmSendMessageType.MULTIPACKET_RESEND_PACKETS);
            ((PumpEnvelope)((MedtronicMessage2)msg.Message2).Message).Message.Message = new MissingSegmentRequest(startPacket, packetCount);

            return msg;
        }

        public AstmStart GetEndEHSM()
        {

            return GetPumpEnvelope(AstmSendMessageType.HIGH_SPEED_MODE_COMMAND);
        }

        public AstmStart GetCloseConnectionRequest()
        {
            AstmStart msg = GetNewRequest((byte)SessionVariables.GetNextSessionNumber(), (byte)AstmCommandType.CLOSE_CONNECTION);
            ((MedtronicMessage2)msg.Message2).Message = new ConnectionRequest(SessionDevice.Device.HMACbyte);
            return msg;
        }

        #endregion

        public void ReadPythonLog(string path)
        {
            PythonLogFileReader reader = new PythonLogFileReader(path, this);
            this.PumpDataHistory.ExtractHistoryEvents();
        }

        public void LoadLog(string path)
        {
            LogFileReader file = new LogFileReader(path, this);
            //this.PumpDataHistory.ExtractHistoryEvents();
        }


       
    }
}
