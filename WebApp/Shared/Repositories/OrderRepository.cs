using Shared.Domains;
using Shared.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Repositories
{
    public interface IOrderRepository : IRepoGenerate<Order>
    {

    }
    public class OrderRepository : RepoGenerate<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
