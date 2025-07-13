using ApiService.Services;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Shared.Domains;

using Shared.Dto;
using Shared.Requests.Commands;
using Shared.Requests.Queries;

namespace ApiService.Handler.Queries
{
    public class CheckOutHandler : IRequestHandler<SearchQuery, BaseRes<PagedResult<OrderDetailDto>>>
    {
        private readonly ICheckOutService _checkOutService;
        private readonly ILogger<CheckOutHandler> _logger;
        private readonly IMapper _mapper;
        public CheckOutHandler(ICheckOutService checkOutService,
            IMapper mapper,
            ILogger<CheckOutHandler> logger)
        {
            _mapper = mapper;
            _checkOutService = checkOutService;
            _logger = logger;
        }

        public async Task<BaseRes<PagedResult<OrderDetailDto>>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var lst = await _checkOutService.SearchPaging(request.SearchText, request.PageIndex, request.PageSize);

                var dtos = lst.Result.ProjectTo<OrderDetailDto>(_mapper.ConfigurationProvider).ToList();

                return new BaseRes<PagedResult<OrderDetailDto>>()
                {
                    Data = new PagedResult<OrderDetailDto>()
                    {
                        Count = lst.Count,
                        Result = dtos
                    },
                    Status = Shared.Enums.EResStatus.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchQuery exception: ");
                return new BaseRes<PagedResult<OrderDetailDto>>()
                {
                    Status = Shared.Enums.EResStatus.SystemError
                };
            }
        }
    }
}
