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

namespace CGM.Communication.Common.Serialize
{
    public class SerializerSession
    {
        protected ILogger Logger = ApplicationLogging.CreateLogger<SerializerSession>();
        private char fieldDelimeter = ';';
        private char keyvalueDelimeter = ':';

        private byte[] _encryptKey;

        public byte[] EncryptKey
        {
            get
            {
                if (_encryptKey == null)
                {
                    if (string.IsNullOrEmpty(this.Device.SerialNumberFull))
                    {
                        throw new ArgumentException("Missing SerialNumberFull to set EncryptKey");
                    }
                    _encryptKey = GetKey(this.Device.SerialNumberFull);
                }
                return _encryptKey;
            }
            set { _encryptKey = value; }
        }

        public bool NeedResetCommunication { get; set; }


        public byte[] EncryptIV
        {
            get
            {
                if (this.EncryptKey != null)
                {
                    byte[] encryptIV = new byte[EncryptKey.Length];
                    Array.Copy(EncryptKey, 0, encryptIV, 0, EncryptKey.Length);
                    encryptIV[0] = RadioChannel;
                    return encryptIV;
                }
                return null;
            }
        }

        public string PumpSerialNumber { get; set; }

        public byte[] LinkMac { get; set; }
        public byte[] PumpMac { get; set; }

        public BayerStickInfoResponse Device { get; set; } = new BayerStickInfoResponse();

        public SessionVariables SessionVariables { get; set; } = new SessionVariables();

        public PumpCarbRatioResponse CarbRatio { get; set; }
        public DeviceCharacteristicsResponse DeviceCharacteristics { get; set; }

        public DeviceStringResponse DeviceString { get; set; }
        public BolusWizardBGTargetsResponse BolusWizardBGTargets { get; set; }
        public BolusWizardSensitivityFactorsResponse BolusWizardSensitivityFactors { get; set; }
        public byte RadioChannel { get; set; }
        public bool RadioChannelConfirmed { get; set; }
        public int RadioRSSI { get; set; }
        public int UploaderBattery { get; set; }

        public bool CanSaveSession
        {
            get
            {
                return !string.IsNullOrEmpty(this.Device.SerialNumber);
            }
        }

        public Configuration Settings { get; set; } = new Configuration();

        public List<PumpStatusMessage> Status { get; set; } = new List<PumpStatusMessage>();

        public List<object> All { get; set; } = new List<object>();

        public List<PumpPattern> PumpPatterns { get; set; } = new List<PumpPattern>();

        public PumpTimeMessage PumpTime { get; set; } = new PumpTimeMessage();

        public List<PumpMessageStartResponse> GeneralMessages { get; set; } = new List<PumpMessageStartResponse>();


        public PumpDataHistory PumpDataHistory { get; set; }

        

        public byte[] LinkKey { get; set; }

        public DateTime? NextRun { get; set; }

        public DateTime? OptimalNextRead { get; set; }
        public DateTime? OptimalNextReadInPumpTime { get; set; }

        public SessionOptions Options { get; set; } = new SessionOptions();

        public SerializerSession()
        {
            NewSession();

        }

        public SerializerSession(SessionOptions options) : this()
        {
            this.Options = options;
        }

        public void NewSession()
        {
            this.SessionVariables = new SessionVariables();
            this.Status = new List<PumpStatusMessage>();
            this.PumpDataHistory = new PumpDataHistory(this);
            this.PumpPatterns = new List<PumpPattern>();
            this.OptimalNextRead = null;
            this.OptimalNextReadInPumpTime = null;
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
                this.OptimalNextReadInPumpTime = optimalInPumptime;
                this.OptimalNextRead = optimalInPumptime.Add(PumpDiffTime);
            }
        }



        public string GetParametersAsString()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add(nameof(RadioChannel), BitConverter.ToString(new byte[] { this.RadioChannel }));
            parameters.Add(nameof(LinkMac), BitConverter.ToString(this.LinkMac));
            parameters.Add(nameof(PumpMac), BitConverter.ToString(this.PumpMac));
            parameters.Add(nameof(LinkKey), BitConverter.ToString(this.LinkKey));
            parameters.Add(nameof(EncryptKey), BitConverter.ToString(this.EncryptKey));
            parameters.Add(nameof(Device.SerialNumber), this.Device.SerialNumber);
            parameters.Add(nameof(Device.ModelNumber), this.Device.ModelNumber);
            parameters.Add(nameof(Device.SerialNumberFull), this.Device.SerialNumberFull);
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

            this.RadioChannel = parameters[nameof(RadioChannel)].GetBytes()[0];

            this.LinkMac = parameters[nameof(LinkMac)].GetBytes();
            this.PumpMac = parameters[nameof(PumpMac)].GetBytes();
            this.LinkKey = parameters[nameof(LinkKey)].GetBytes();
            this.Device.SerialNumber = parameters[nameof(Device.SerialNumber)];
            this.Device.ModelNumber = parameters[nameof(Device.ModelNumber)];
            this.Device.SerialNumberFull = parameters[nameof(Device.SerialNumberFull)];
        }



        public byte[] GetKey(string serialnumber)
        {
            if (this.LinkKey == null || serialnumber == null)
            {
                throw new ArgumentException("Missing Linkkey/number to set EncryptKey");
            }
            byte[] PackedLinkKey = this.LinkKey;

            var key = new byte[16];
            int pos = serialnumber[serialnumber.Length - 1] & 7;
            for (int i = 0; i < key.Length; i++)
            {
                if ((PackedLinkKey[pos + 1] & 1) == 1)
                {
                    key[i] = (byte)~PackedLinkKey[pos];
                }
                else
                {
                    key[i] = PackedLinkKey[pos];
                }

                if (((PackedLinkKey[pos + 1] >> 1) & 1) == 0)
                {
                    pos += 3;
                }
                else
                {
                    pos += 2;
                }
            }
            return key;

        }


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
            ((MedtronicMessage2)msg.Message2).Message = new ConnectionRequest(Device.HMACbyte);
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
            ((MedtronicMessage2)msg.Message2).Message = new RadioChannelRequest(radioChannel, LinkMac, PumpMac);
            return msg;
        }

        private AstmStart GetPumpEnvelope(byte prefix, AstmSendMessageType messageType, byte[] message)
        {
            AstmStart msg = GetNewRequest((byte)SessionVariables.GetNextSessionNumber(), (byte)AstmCommandType.SEND_MESSAGE);
            PumpEnvelope penv = new PumpEnvelope(this.PumpMac, (byte)SessionVariables.GetNextSequenceNumber());
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

        public AstmStart GetPumpBasalPattern(int patternNumber)
        {
            return GetPumpEnvelope(AstmSendMessageType.READ_BASAL_PATTERN_REQUEST, BitConverter.GetBytes(patternNumber));
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
            var lastRead = this.Settings.LastRead.FirstOrDefault(e => e.DataType == (int)historyDataType);
            int lastRtc = 0;
            if (lastRead == null)
            {
                lastRtc = DateTime.Now.AddDays(-1 * this.Settings.HistoryDaysBack).GetRtcBytes(this.PumpTime.OffSet).GetInt32(0);
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
                ((PumpEnvelope)((MedtronicMessage2)msg.Message2).Message).Message.Message = new ReadHistoryRequest(lastRtc,historyDataType);
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
            ((MedtronicMessage2)msg.Message2).Message = new ConnectionRequest(Device.HMACbyte);
            return msg;
        }
    }
}
