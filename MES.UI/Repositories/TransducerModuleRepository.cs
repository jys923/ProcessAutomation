﻿using MES.UI.Models;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class TransducerModuleRepository : RepositoryBase<TransducerModule>, ITransducerModuleRepository
    {
        public TransducerModuleRepository(DbContext context) : base(context)
        {
        }
    }
}
