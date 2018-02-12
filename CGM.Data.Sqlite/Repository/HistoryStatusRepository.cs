using System;
using System.Collections.Generic;
using System.Text;

namespace  CMG.Data.Sqlite.Repository
{
    public class HistoryStatusRepository : BaseRepository<HistoryStatus>
    {
        public HistoryStatusRepository(CgmUnitOfWork uow) : base(uow)
        {
        }

        public void AddKeys(List<string> list, int status)
        {
            List<HistoryStatus> statusList = new List<HistoryStatus>();
            list.ForEach(e => statusList.Add(new HistoryStatus() { Key = e,  Status = status }));
            this.AddRange(statusList);
        }
    }
}
