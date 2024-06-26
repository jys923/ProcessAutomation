﻿using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    public interface IProbeSNRepository : IRepositoryBase<TransducerModule>
    {
        IEnumerable<TransducerModuleView> GetProbeSN(int resultCnt);
    }
}