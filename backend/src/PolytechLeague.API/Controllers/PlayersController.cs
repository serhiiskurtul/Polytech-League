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
    private readonly IFileService _fileService;

    public PlayersController(IPlayerService playerService, IUserService userService, IFileService fileService)
    {
        _playerService = playerService;
        _userService = userService;
        _fileService = fileService;
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

    [Authorize]
    [HttpPost("profile/avatar")]
    public async Task<ActionResult<string>> UploadAvatar([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);

        if (player == null) return NotFound("Player not found");

        // Save new file
        var photoUrl = await _fileService.SaveFileAsync(file, "avatars", cancellationToken);

        // Delete old file if it exists and is not a default one
        if (!string.IsNullOrEmpty(player.PhotoUrl) && player.PhotoUrl.StartsWith("/uploads/avatars/"))
        {
            try { _fileService.DeleteFile(player.PhotoUrl); } catch { /* ignore */ }
        }

        // Update player
        player.PhotoUrl = photoUrl;
        await _playerService.UpdateAsync(player.Id ?? 0, player, cancellationToken);

        return Ok(new { photoUrl });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] PlayerDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);
        if (player == null) return NotFound("Player profile not found");
        
        await _playerService.UpdateAsync(player.Id ?? 0, dto, cancellationToken);
        return NoContent();
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

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create(PlayerDto dto, CancellationToken cancellationToken)
    {
        var created = await _playerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, PlayerDto dto, CancellationToken cancellationToken)
    {
        await _playerService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
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

    [HttpGet("rankings")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetRankings(CancellationToken cancellationToken)
    {
        try {
            var results = await _playerService.GetAllWithRankingsAsync(cancellationToken);
            return Ok(results);
        } catch (Exception ex) {
            Console.WriteLine($"CONTROLLER RANKINGS ERROR: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("check-archival")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> CheckArchival([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] string group, CancellationToken cancellationToken)
    {
        var matches = await _playerService.FindPotentialArchivalMatchesAsync(firstName, lastName, group, cancellationToken);
        return Ok(matches);
    }

    [Authorize]
    [HttpPost("merge-request")]
    public async Task<IActionResult> CreateMergeRequest([FromQuery] long targetPlayerId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _playerService.CreateMergeRequestAsync(userId, targetPlayerId, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("merge-requests")]
    public async Task<ActionResult<IEnumerable<ProfileMergeRequestDto>>> GetMergeRequests(CancellationToken cancellationToken)
    {
        var requests = await _playerService.GetPendingMergeRequestsAsync(cancellationToken);
        return Ok(requests);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("merge-requests/{id}/approve")]
    public async Task<IActionResult> ApproveMergeRequest(long id, CancellationToken cancellationToken)
    {
        await _playerService.ApproveMergeRequestAsync(id, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("merge-requests/{id}/reject")]
    public async Task<IActionResult> RejectMergeRequest(long id, [FromQuery] string comment, CancellationToken cancellationToken)
    {
        await _playerService.RejectMergeRequestAsync(id, comment, cancellationToken);
        return Ok();
    }
}

