using Shared.Domains;
using Shared.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Repositories
{
    public interface IProductRepository : IRepoGenerate<Product>
    {

    }
    public class ProductRepository : RepoGenerate<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
