using CGM.Communication.Common.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CGM.Communication.MiniMed.Infrastructur;

namespace CGM.Communication.Data.Repository
{
    public class CommunicationMessageRepository : BaseRepository<CommunicationMessage>
    {
        public CommunicationMessageRepository(CgmUnitOfWork uow) : base(uow)
        {
        }

        public void SaveSession(SerializerSession session)
        {
            List<CommunicationMessage> msgs = new List<CommunicationMessage>();

            foreach (var handler in session.PumpDataHistory.MultiPacketHandlers)
            {
                foreach (var item in handler.Segments)
                {
                    foreach (var eve in item.Events)
                    {
                        CommunicationMessageTemp msg = new CommunicationMessageTemp();
                        msg.MessageKey = $"{handler.ReadInfoResponse.HistoryDataTypeRaw} - {BitConverter.ToString(eve.AllBytes)}";
                        msg.MessageDateTime = eve.Timestamp.Value;
                        msg.MessageType = handler.ReadInfoResponse.HistoryDataType.ToString();
                        msg.MessageSubType = eve.EventType.ToString();
                        msg.Message = BitConverter.ToString(eve.AllBytes);

                        if (eve.EventType == EventTypeEnum.ALARM_NOTIFICATION)
                        {
                            msg.NotificationStatus = 1;
                        }

                        if (eve.EventType == EventTypeEnum.SENSOR_GLUCOSE_READINGS_EXTENDED
                            ||
                            eve.EventType == EventTypeEnum.BOLUS_WIZARD_ESTIMATE
                            ||
                            eve.EventType == EventTypeEnum.ALARM_NOTIFICATION
                            ||
                            eve.EventType == EventTypeEnum.GLUCOSE_SENSOR_CHANGE
                            )
                        {
                            msg.NightScoutStatus = 1;
                        }

                        msgs.Add(msg);
                    }

                }
            }

            //this._uow.Connection.DeleteAll<CommunicationMessageTemp>();
            //this._uow.Connection.InsertAll(msgs,false);

            var listToAdd = this._uow.Connection.Query<CommunicationMessage>("select * from CommunicationMessage right outer join CommunicationMessageTemp on CommunicationMessage.MessageKey=CommunicationMessageTemp.MessageKey where CommunicationMessage.MessageKey is null", null);

            this._uow.Connection.DeleteAll<CommunicationMessageTemp>();
            this._uow.Connection.InsertAll(listToAdd);
       
        }

        //private void AddList(List<CommunicationMessage> msgs)
        //{
        //    var query = _uow.Connection.Table<CommunicationMessage>().Where(v => v.MessageKey);
        //    CommunicationMessage setting = query.FirstOrDefault();
        //}
    }


    public class CommunicationMessageTemp : CommunicationMessage
    {

    }
}
