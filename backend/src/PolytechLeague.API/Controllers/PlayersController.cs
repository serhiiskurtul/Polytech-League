using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IUserService _userService;

    public PlayersController(IPlayerService playerService, IUserService userService)
    {
        _playerService = playerService;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<PlayerDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);

        if (player == null)
        {
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            player = new PlayerDto
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Group = "Гість", // Default value
                City = "Київ"
            };
            player = await _playerService.CreateAsync(player, cancellationToken);
        }

        return Ok(player);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var players = await _playerService.GetAllAsync(cancellationToken);
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var player = await _playerService.GetByIdAsync(id, cancellationToken);
        if (player == null) return NotFound();
        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create(PlayerDto dto, CancellationToken cancellationToken)
    {
        var created = await _playerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, PlayerDto dto, CancellationToken cancellationToken)
    {
        await _playerService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await _playerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> Search([FromQuery] string query, CancellationToken cancellationToken)
    {
        var results = await _playerService.SearchAsync(query, cancellationToken);
        return Ok(results);
    }
}
