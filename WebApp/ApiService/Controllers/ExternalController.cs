using Microsoft.AspNetCore.Mvc;
using MediatR;
using Shared.Requests.Commands;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly ILogger<ExternalController> _logger;
        private readonly IMediator _mediator;

        public ExternalController(ILogger<ExternalController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("product")]
        [HttpPost]
        public async Task<ActionResult<string>> product([FromBody] string id)
        {
            var res = await _mediator.Send(new ExeProductCmd()
            {
                ProductId = id
            });

            return !string.IsNullOrEmpty(res) ? Ok() : NotFound();
        }

        [Route("email")]
        [HttpPost]
        public async Task<ActionResult<string>> email([FromBody] string id)
        {
            var res = await _mediator.Send(new ExeEmailCmd()
            {
                EmailId = id
            });

            return !string.IsNullOrEmpty(res) ? Ok() : NotFound();
        }

        [Route("payment")]
        [HttpPost]
        public async Task<ActionResult<string>> payment([FromBody] string price)
        {
            var res = await _mediator.Send(new PaymentCmd()
            {
                Price = int.Parse(price)
            });

            return !string.IsNullOrEmpty(res) ? Ok() : NotFound();
        }
    }
}
