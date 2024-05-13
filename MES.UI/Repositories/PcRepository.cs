using MES.UI.Models;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.Repositories
{
    public class PcRepository : RepositoryBase<Pc> , IPcRepository
    {
        public PcRepository(DbContext context) : base(context)
        {
        }
    }
}
