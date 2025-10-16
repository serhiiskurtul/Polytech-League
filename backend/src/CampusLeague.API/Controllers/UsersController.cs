using CampusLeague.Application.Dto;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Domain.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


[ApiController]
[Route("users")]
public class UsersController(
    IUserService userService) : Controller
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var users = await userService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<UserResponse>
        (
            data: users.Data.Select(d => d.ToResponse()).ToList(),
            totalCount: users.TotalCount
        );

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("with-deleted")]
    public async Task<IActionResult> GetAllWithDeleted(
        [FromRoute] SortParams? sortParams, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllWithDeletedAsync(sortParams, cancellationToken);
        var usersResponse = users.Select(u => u.ToResponseWithDeleted());
        return Ok(usersResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        var users = await userService.GetByIdAsync(id, cancellationToken);
        var usersResponse = users.ToResponse();
        return Ok(usersResponse);
    }

    [EnableRateLimiting("fixed")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = userRequest.FromRequest();
        await userService.CreateAsync(user, cancellationToken);
    
        return Created();
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id, [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = userRequest.FromRequest();
        await userService.UpdateAsync(id, user, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "CustomerOrAdmin")]
    [HttpDelete("soft-delete/{id}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.SoftDeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("hard-delete/{id}")]
    public async Task<IActionResult> HardDelete(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
