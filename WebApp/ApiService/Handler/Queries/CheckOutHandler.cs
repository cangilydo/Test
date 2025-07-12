using ApiService.Services;

using MediatR;

using Shared.Domains;

using Shared.Dto;
using Shared.Requests.Commands;
using Shared.Requests.Queries;

namespace ApiService.Handler.Queries
{
    public class CheckOutHandler : IRequestHandler<SearchQuery, PagedResult<V_OrderDetail>>
    {
        private readonly ICheckOutService _checkOutService;
        private readonly ILogger<CheckOutHandler> _logger;
        public CheckOutHandler(ICheckOutService checkOutService,
            ILogger<CheckOutHandler> logger)
        {
            _checkOutService = checkOutService;
            _logger = logger;
        }

        public async Task<PagedResult<V_OrderDetail>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var lst = await _checkOutService.SearchPaging(request.SearchText, request.PageIndex, request.PageSize);
                return lst;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchQuery exception: ");
                return null;
            }
        }
    }
}
