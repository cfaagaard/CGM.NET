using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.MiniMed.Responses.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.DTO
{
    public static class DtoFactory
    {
        public static SessionDTO GetSessionDTO(this SerializerSession serializeSession)
        {

            SessionDTO sessionDTO = new SessionDTO();

            
            serializeSession.PumpDataHistory.PumpEvents.ForEach(e => sessionDTO.PumpEvents.Add(e.AllBytes));
            serializeSession.PumpDataHistory.SensorEvents.ForEach(e => sessionDTO.SensorEvents.Add(e.AllBytes));

            sessionDTO.BolusWizardBGTargets = serializeSession.PumpSettings.BolusWizardBGTargets.BytesAsString.GetBytes();
            sessionDTO.BolusWizardSensitivityFactors = serializeSession.PumpSettings.BolusWizardSensitivityFactors.BytesAsString.GetBytes();
            sessionDTO.CarbRatio = serializeSession.PumpSettings.CarbRatio.BytesAsString.GetBytes();
            sessionDTO.DeviceCharacteristics = serializeSession.PumpSettings.DeviceCharacteristics.BytesAsString.GetBytes();
            sessionDTO.PumpTime = serializeSession.PumpTime.AllBytes;

            serializeSession.PumpSettings.PumpPatterns.ForEach(e => sessionDTO.PumpPatterns.Add(e.BytesAsString.GetBytes()));
            sessionDTO.Device = serializeSession.SessionDevice.Device.AllBytes;
            serializeSession.Status.ForEach(e => sessionDTO.Status.Add(e.AllBytes));

            return sessionDTO;

        }

        public static SerializerSession GetSerializerSession(this SessionDTO sessionDTO)
        {

            SerializerSession session = new SerializerSession();
            Serializer serializer = new Serializer(session);

            serializer.Deserialize<PumpTimeMessage>(sessionDTO.PumpTime);

            session.SessionDevice.Device = serializer.Deserialize<BayerStickInfoResponse>(sessionDTO.Device);
            session.PumpSettings.DeviceCharacteristics = serializer.Deserialize<DeviceCharacteristicsResponse>(sessionDTO.DeviceCharacteristics);
            session.PumpSettings.BolusWizardBGTargets = serializer.Deserialize<BolusWizardBGTargetsResponse>(sessionDTO.BolusWizardBGTargets);
            session.PumpSettings.BolusWizardSensitivityFactors = serializer.Deserialize<BolusWizardSensitivityFactorsResponse>(sessionDTO.BolusWizardSensitivityFactors);
            session.PumpSettings.CarbRatio = serializer.Deserialize<PumpCarbRatioResponse>(sessionDTO.CarbRatio);
            sessionDTO.PumpPatterns.ForEach(e => serializer.Deserialize<PumpPattern>(e));

            sessionDTO.Status.ForEach(e => serializer.Deserialize<PumpStatusMessage>(e));

            sessionDTO.PumpEvents.ForEach(e => session.PumpDataHistory.PumpEvents.Add(serializer.Deserialize<BasePumpEvent>(e)));
            sessionDTO.SensorEvents.ForEach(e => session.PumpDataHistory.SensorEvents.Add(serializer.Deserialize<BasePumpEvent>(e)));

         
            
            
            


            return session;

        }
    }
}
