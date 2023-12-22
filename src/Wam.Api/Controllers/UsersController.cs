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
}