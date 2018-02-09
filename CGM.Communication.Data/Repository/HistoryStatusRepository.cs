using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data.Repository
{
    public class HistoryStatusRepository : BaseRepository<HistoryStatus>
    {
        public HistoryStatusRepository(CgmUnitOfWork uow) : base(uow)
        {
        }

        internal void AddKeys(List<string> list, HistoryStatusTypeEnum historyStatusType,int status)
        {
            List<HistoryStatus> statusList = new List<HistoryStatus>();
            list.ForEach(e => statusList.Add(new HistoryStatus() { Key = e, HistoryStatusType = (int)historyStatusType, Status = status }));
            this.AddRange(statusList);
        }
    }
}
