using Microsoft.EntityFrameworkCore;
using System.Linq;

using Shared.Domains;
using Shared.EF;
using Shared.Extensions;
using Shared.Dto;

namespace Shared.Repositories
{
    public interface IV_OrderDetailRepository : IRepoGenerate<V_OrderDetail>
    {
        Task<PagedQueryAbleResult<V_OrderDetail>> SearchPaging(string searchText, int pageIndex, int pageSize);
    }
    public class V_OrderDetailRepository : RepoGenerate<V_OrderDetail>, IV_OrderDetailRepository
    {
        public V_OrderDetailRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedQueryAbleResult<V_OrderDetail>> SearchPaging(string searchText, int pageIndex, int pageSize)
        {            
            searchText = string.IsNullOrEmpty(searchText) ? string.Empty : ConvertHelper.ConvertUnicode(searchText.Trim());
            var res = string.IsNullOrEmpty(searchText) ? GetAll() :
                _dbContext.V_OrderDetails.FromSqlInterpolated($@"
                SELECT ""ProductId"",""OrderId"",""Name"",""NormalizeName"", ""Status"", ""CreatedOn""
                FROM public.""V_OrderDetails""
                WHERE ""NameVector"" @@ plainto_tsquery('simple', {searchText})
            ");

            var total = await res.CountAsync();

            var items = await res
                .OrderByDescending(p => p.CreatedOn)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedQueryAbleResult<V_OrderDetail>()
            {
                Count = total,
                Result = items.AsQueryable()
            };
        }
    }
}
