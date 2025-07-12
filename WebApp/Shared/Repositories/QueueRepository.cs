using Shared.Domains;
using Shared.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Repositories
{
    public interface IQueueRepository : IRepoGenerate<Queue>
    {

    }
    public class QueueRepository : RepoGenerate<Queue>, IQueueRepository
    {
        public QueueRepository(AppDbContext context) : base(context)
        {
        }
    }
}
