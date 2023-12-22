using Microsoft.AspNetCore.Mvc;
using Wam.Users.DataTransferObjects;
using Wam.Users.Services;

namespace Wam.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersService usersService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserDetailsDto>> Post([FromBody] UserCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await usersService.Create(dto, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDetailsDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await usersService.Get(id, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id:guid}/Ban/{reason?}")]
    public async Task<ActionResult<UserDetailsDto>> Ban(Guid id, CancellationToken cancellationToken, byte? reason = 2)
    {
        var result = await usersService.Ban(id, reason??2,cancellationToken);
        return Ok(result);
    }
}