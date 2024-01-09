using Microsoft.AspNetCore.Mvc;

namespace Wam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpGet("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(string code, CancellationToken cancellationToken)
        {
            return Ok();
        }
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpGet("{id:guid}/join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Join(Guid id, CancellationToken cancellationToken)
        {
            return Ok();
        }

    }
}
