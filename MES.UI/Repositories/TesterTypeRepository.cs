﻿using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class TesterTypeRepository : RepositoryBase<Tester> , ITesterTypeRepository
    {
        public TesterTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
