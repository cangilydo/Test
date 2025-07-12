using Shared.Domains;
using Shared.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Repositories
{
    public interface IAuditRepository : IRepoGenerate<Audit>
    {

    }
    public class AuditRepository : RepoGenerate<Audit>, IAuditRepository
    {
        public AuditRepository(AppDbContext context) : base(context)
        {
        }
    }
}
