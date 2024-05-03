using MES.UI.Models;
using MES.UI.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.Repositories
{
    public class ProbeRepository : RepositoryBase<Probe>
    {
        public ProbeRepository(DbContext context) : base(context)
        {
        }
    }
}
