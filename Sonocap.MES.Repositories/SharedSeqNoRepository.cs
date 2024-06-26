using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class SharedSeqNoRepository : RepositoryBase<SharedSeqNo>, ISharedSeqNoRepository
    {
        public SharedSeqNoRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        { 
        }

        public async Task InitializeAsync()
        {
            SharedSeqNo? seqNo = await GetSeqNoAsync();
            if (seqNo == null)
            {
                await InsertAsync(new SharedSeqNo());
            }
        }

        public async Task<SharedSeqNo?> GetSeqNoAsync(DateTime dateTime = default)
        {
            dateTime = dateTime == default ? DateTime.Today : dateTime;
            
            var query = from SeqNo in _context.Set<SharedSeqNo>()
                        where SeqNo.Date == dateTime
                        select SeqNo;

            SharedSeqNo? seqNo = await query.FirstOrDefaultAsync();

            return seqNo;
        }

        public async Task<bool> SetSeqNoAsync(DateTime dateTime = default)
        {
            dateTime = dateTime == default ? DateTime.Today : dateTime;

            var query = from SeqNo in _context.Set<SharedSeqNo>()
                        where SeqNo.Date.Date == dateTime.Date
                        select SeqNo;

            SharedSeqNo? seqNo = query.FirstOrDefault();
            if (seqNo != null)
            {
                seqNo.TDMdNo++;
                seqNo.ProbeNo++;
                _context.Update(seqNo);
            }

            int count = await _context.SaveChangesAsync();

            return count > 0;
        }

        public async Task<bool> SetSeqNoAsync(SnType type, DateTime dateTime = default)
        {
            dateTime = dateTime == default ? DateTime.Today : dateTime;

            var query = from SeqNo in _context.Set<SharedSeqNo>()
                        where SeqNo.Date.Date == dateTime.Date
                        select SeqNo;

            SharedSeqNo? seqNo = query.FirstOrDefault();
            if (seqNo != null)
            {
                switch (type)
                {
                    case SnType.TransducerModule:
                        seqNo.TDMdNo++;
                        break;
                    case SnType.Probe:
                        seqNo.ProbeNo++;
                        break;

                }
                _context.Update(seqNo);
            }

            int count = await _context.SaveChangesAsync();

            return count > 0;
        }

        public async Task<SharedSeqNo?> UpsertSeqNoAsync(SnType type, DateTime dateTime = default)
        {
            dateTime = dateTime == default ? DateTime.Today : dateTime;

            var query = from SeqNo in _context.Set<SharedSeqNo>()
                        where SeqNo.Date.Date == dateTime.Date
                        select SeqNo;

            SharedSeqNo? seqNo = query.FirstOrDefault();
            if (seqNo != null)
            {
                switch (type)
                {
                    case SnType.TransducerModule:
                        seqNo.TDMdNo++;
                        break;
                    case SnType.Probe:
                        seqNo.ProbeNo++;
                        break;
                }
                _context.Update(seqNo);
            }
            else
            {
                // seqNo가 없는 경우 새로운 seqNo를 생성하고 추가합니다.
                seqNo = new SharedSeqNo { Date = dateTime };
                await _context.AddAsync(seqNo);
            }

            int count = await _context.SaveChangesAsync();

            if (count < 1)
            {
                seqNo = null;
            }

            return seqNo;
        }
    }
}
