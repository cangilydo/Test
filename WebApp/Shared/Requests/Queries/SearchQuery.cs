using MediatR;

using Shared.Domains;
using Shared.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Requests.Queries
{
    public class SearchQuery :  IRequest<PagedResult<V_OrderDetail>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }
}
