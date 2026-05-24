using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PolytechLeague.Application.Extensions;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Account;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, IPlayerService playerService, IRedisService redisService) : Controller
{
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var users = await userService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<UserResponse>(
            data: users.Data.Select(d => d.ToResponse()).ToList(),
            totalCount: users.TotalCount);

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("with-deleted")]
    public async Task<IActionResult> GetAllWithDeleted(
        [FromRoute] SortParams? sortParams, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllWithDeletedAsync(sortParams, cancellationToken);
        var usersResponse = users.Select(u => u.ToResponseWithDeleted());
        return Ok(usersResponse);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        var users = await userService.GetByIdAsync(id, cancellationToken);
        return Ok(users.ToResponse());
    }

    [AllowAnonymous]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Ok(null);

        var userId = User.GetUserId();
        var cacheKey = $"user_account_{userId}";

        var cached = await redisService.GetDataAsync<UserResponse>(cacheKey);
        if (cached != null) return Ok(cached);

        var user = await userService.GetByIdAsync(userId, cancellationToken);
        var response = user.ToResponse();

        // Get photo from player profile if exists (lightweight check)
        response.PhotoUrl = await playerService.GetPhotoUrlByUserIdAsync(userId, cancellationToken);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromHours(24), "user_accounts_group");

        return Ok(response);
    }

    [EnableRateLimiting("fixed")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = userRequest.FromRequest();
        await userService.CreateAsync(user, cancellationToken);
        return StatusCode(201);
    }

    [Authorize(Policy = "User")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id, [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = userRequest.FromRequest();
        await userService.UpdateAsync(id, user, cancellationToken);

        var cacheKey = $"user_account_{id}";
        await redisService.RemoveDataAsync(cacheKey);

        return NoContent();
    }

    [Authorize(Policy = "CustomerOrAdmin")]
    [HttpDelete("soft-delete/{id}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.SoftDeleteAsync(id, cancellationToken);

        var cacheKey = $"user_account_{id}";
        await redisService.RemoveDataAsync(cacheKey);

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("hard-delete/{id}")]
    public async Task<IActionResult> HardDelete(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);

        var cacheKey = $"user_account_{id}";
        await redisService.RemoveDataAsync(cacheKey);

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("block/{id}")]
    public async Task<IActionResult> Block(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.BlockAsync(id, cancellationToken);

        var cacheKey = $"user_account_{id}";
        await redisService.RemoveDataAsync(cacheKey);

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("unblock/{id}")]
    public async Task<IActionResult> Unblock(
        [FromRoute] long id, CancellationToken cancellationToken)
    {
        await userService.UnblockAsync(id, cancellationToken);

        var cacheKey = $"user_account_{id}";
        await redisService.RemoveDataAsync(cacheKey);

        return NoContent();
    }
}
