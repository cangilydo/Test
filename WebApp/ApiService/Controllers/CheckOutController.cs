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
        public async Task<ActionResult<string>> Process([FromBody] CheckOutCmd request)
        {
            var res = await _mediator.Send(request);

            return !string.IsNullOrEmpty(res) ? Ok() : NotFound();
        }

        [Route("list")]
        [HttpPost]
        public async Task<ActionResult<PagedResult<V_OrderDetail>>> List([FromBody] SearchQuery request)
        {
            var res = await _mediator.Send(request);

            return res;
        }
    }
}
