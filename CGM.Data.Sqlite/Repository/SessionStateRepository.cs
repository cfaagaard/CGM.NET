using System;
using System.Collections.Generic;
using System.Text;
using CGM.Communication.Common.Serialize;

namespace  CMG.Data.Sqlite.Repository
{
    public class SessionStateRepository : CGM.Communication.Interfaces.IStateRepository
    {
        public void AddUpdateSessionToDevice(SerializerSession session)
        {
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                uow.Device.AddUpdateSessionToDevice(session);
            }
        }

        public void GetOrSetSessionAndSettings(SerializerSession session)
        {
            using (CgmUnitOfWork uow = new CgmUnitOfWork())
            {
                uow.Device.GetOrSetSessionAndSettings(session);
            }
        }
    }
}
