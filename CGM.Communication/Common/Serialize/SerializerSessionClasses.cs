using CGM.Communication.MiniMed.Responses;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Common.Serialize
{
    public class SessionSystem
    {
        public bool PreserveMessages { get; set; } = false;
        public List<object> Messages { get; set; } = new List<object>();
        public List<PumpMessageStartResponse> GeneralMessages { get; set; } = new List<PumpMessageStartResponse>();

        public DateTime? NextRun { get; set; }

        public DateTime? OptimalNextRead { get; set; }
        public DateTime? OptimalNextReadInPumpTime { get; set; }

        public void NewSession()
        {
            this.OptimalNextRead = null;
            this.OptimalNextReadInPumpTime = null;
            this.Messages = new List<object>();
            this.GeneralMessages = new List<PumpMessageStartResponse>();
        }
    }

    public class SessionDevice
    {


        public BayerStickInfoResponse Device { get; set; } = new BayerStickInfoResponse();
    }

    public class SessionCommunicationParameters
    {
        public bool NeedResetCommunication { get; set; }
        public byte[] LinkKey { get; set; }
        public byte[] LinkMac { get; set; }
        public byte[] PumpMac { get; set; }
        public byte RadioChannel { get; set; }
        public bool RadioChannelConfirmed { get; set; }
        public int RadioRSSI { get; set; }

        private byte[] _encryptKey;

        public byte[] EncryptKey
        {
            get
            {
                if (_encryptKey == null)
                {
                    if (string.IsNullOrEmpty(_session.SessionDevice.Device.SerialNumberFull))
                    {
                        throw new ArgumentException("Missing SerialNumberFull to set EncryptKey");
                    }
                    _encryptKey = GetKey(_session.SessionDevice.Device.SerialNumberFull);
                }
                return _encryptKey;
            }
            set { _encryptKey = value; }
        }

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
        private SerializerSession _session;
        public SessionCommunicationParameters(SerializerSession session)
        {
            _session = session;
        }

        private byte[] GetKey(string serialnumber)
        {
            if (this.LinkKey == null || serialnumber == null)
            {
                return null;
                //throw new ArgumentException("Missing Linkkey/number to set EncryptKey");
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

    }

    public class PumpSettings
    {
        [BsonId]
        public int Id { get; set; } = 1;
        public PumpCarbRatioResponse CarbRatio { get; set; }

        public BolusWizardBGTargetsResponse BolusWizardBGTargets { get; set; }
        public BolusWizardSensitivityFactorsResponse BolusWizardSensitivityFactors { get; set; }
        public DeviceCharacteristicsResponse DeviceCharacteristics { get; set; }
        public DeviceStringResponse DeviceString { get; set; }
        public List<PumpPattern> PumpPatterns { get; set; } = new List<PumpPattern>();

        public string GetCompareString()
        {
            string compare = "";

            compare += CarbRatio?.BytesAsString;
            compare += BolusWizardBGTargets?.BytesAsString;
            compare += BolusWizardSensitivityFactors?.BytesAsString;
            compare += DeviceCharacteristics?.BytesAsString;

            foreach (var item in PumpPatterns)
            {
                compare += item.BytesAsString;
            }

            return compare;
        }

        public bool IsNew(string OldString)
        {
            return this.GetCompareString() != OldString;
        }
    }
}
