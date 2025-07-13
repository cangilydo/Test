using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Domains;
using Shared.Dto;
using Shared.Requests.Commands;
using Shared.Requests.Queries;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckOutController : ControllerBase
    {
        private readonly ILogger<CheckOutController> _logger;
        private readonly IMediator _mediator;

        public CheckOutController(ILogger<CheckOutController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("process")]
        [HttpPost]
        public async Task<ActionResult<BaseRes<string>>> Process([FromBody] CheckOutCmd request)
        {
            var res = await _mediator.Send(request);

            return res;
        }

        [Route("list")]
        [HttpPost]
        public async Task<ActionResult<BaseRes<PagedResult<OrderDetailDto>>>> List([FromBody] SearchQuery request)
        {
            var res = await _mediator.Send(request);

            return res;
        }
    }
}
