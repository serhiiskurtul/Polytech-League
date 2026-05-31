using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly IRedisService _redisService;

    public PlayersController(IPlayerService playerService, IUserService userService, IFileService fileService, IRedisService redisService)
    {
        _playerService = playerService;
        _userService = userService;
        _fileService = fileService;
        _redisService = redisService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<PlayerDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var cacheKey = $"player_profile_{userId}";
        
        var cached = await _redisService.GetDataAsync<PlayerDto>(cacheKey);
        if (cached != null) return Ok(cached);

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);

        if (player == null)
        {
            player = new PlayerDto
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Group = "Гість", // Default value
                City = "Чернівці",
                PhotoUrl = user.PhotoUrl
            };
            player = await _playerService.CreateAsync(player, cancellationToken);
        }
        else if (string.IsNullOrEmpty(player.PhotoUrl) && !string.IsNullOrEmpty(user.PhotoUrl))
        {
            // Sync photo from user to player if missing
            player.PhotoUrl = user.PhotoUrl;
            await _playerService.UpdateAsync(player.Id ?? 0, player, cancellationToken);
        }

        await _redisService.SetDataAsync(cacheKey, player, TimeSpan.FromHours(24), "player_profiles_group");
        if (player.Id.HasValue)
        {
            var playerCacheKey = $"player_profile_{player.Id.Value}";
            await _redisService.SetDataAsync(playerCacheKey, player, TimeSpan.FromHours(24), "player_profiles_group");
        }
        return Ok(player);
    }

    [Authorize]
    [HttpPost("profile/avatar")]
    public async Task<ActionResult<string>> UploadAvatar([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user == null) return NotFound("User not found");

        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);

        // Save new file to storage
        var photoUrl = await _fileService.SaveFileAsync(file, "avatars", cancellationToken);

        // Delete old file from storage if it exists
        var oldPhotoUrl = user.PhotoUrl ?? player?.PhotoUrl;
        if (!string.IsNullOrEmpty(oldPhotoUrl) && (oldPhotoUrl.StartsWith("/uploads/avatars/") || oldPhotoUrl.Contains("r2.cloudflarestorage.com") || oldPhotoUrl.Contains("r2.dev")))
        {
            try { _fileService.DeleteFile(oldPhotoUrl); } catch { /* ignore */ }
        }

        // 1. Update User entity
        user.PhotoUrl = photoUrl;
        await _userService.UpdateAsync(userId, user, cancellationToken);

        // 2. Update Player entity if it exists (to keep in sync)
        if (player != null)
        {
            player.PhotoUrl = photoUrl;
            await _playerService.UpdateAsync(player.Id ?? 0, player, cancellationToken);
        }

        var cacheKey = $"player_profile_{userId}";
        await _redisService.RemoveDataAsync(cacheKey);

        if (player != null && player.Id.HasValue)
        {
            var playerCacheKey = $"player_profile_{player.Id.Value}";
            await _redisService.RemoveDataAsync(playerCacheKey);
        }

        var userCacheKey = $"user_account_{userId}";
        await _redisService.RemoveDataAsync(userCacheKey);

        return Ok(new { photoUrl });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<PlayerDto>> UpdateProfile([FromBody] PlayerDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var player = await _playerService.GetByUserIdAsync(userId, cancellationToken);
        if (player == null) return NotFound("Player profile not found");
        
        // Ensure we are updating the correct player
        await _playerService.UpdateAsync(player.Id ?? 0, dto, cancellationToken);
        
        // Fetch again to get the most up-to-date data
        var updated = await _playerService.GetByUserIdAsync(userId, cancellationToken);
        
        var cacheKey = $"player_profile_{userId}";
        await _redisService.SetDataAsync(cacheKey, updated, TimeSpan.FromHours(24), "player_profiles_group");

        if (updated != null && updated.Id.HasValue)
        {
            var playerCacheKey = $"player_profile_{updated.Id.Value}";
            await _redisService.SetDataAsync(playerCacheKey, updated, TimeSpan.FromHours(24), "player_profiles_group");
        }

        var userCacheKey = $"user_account_{userId}";
        await _redisService.RemoveDataAsync(userCacheKey);

        return Ok(updated);
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
        var cacheKey = $"player_profile_{id}";
        var cached = await _redisService.GetDataAsync<PlayerDto>(cacheKey);
        if (cached != null) return Ok(cached);

        var player = await _playerService.GetByIdAsync(id, cancellationToken);
        if (player == null) return NotFound();

        await _redisService.SetDataAsync(cacheKey, player, TimeSpan.FromHours(24), "player_profiles_group");
        if (player.UserId.HasValue)
        {
            var userCacheKey = $"player_profile_{player.UserId.Value}";
            await _redisService.SetDataAsync(userCacheKey, player, TimeSpan.FromHours(24), "player_profiles_group");
        }

        return Ok(player);
    }
    [HttpGet("{id}/rank")]
    public async Task<ActionResult<PlayerRankResponse>> GetPlayerRank(long id, CancellationToken cancellationToken)
    {
        var cacheKey = "global_rankings_all";
        var allWithRankings = await _redisService.GetDataAsync<IEnumerable<PlayerDto>>(cacheKey);
        
        if (allWithRankings == null)
        {
            allWithRankings = await _playerService.GetAllWithRankingsAsync(null, cancellationToken);
            await _redisService.SetDataAsync(cacheKey, allWithRankings, TimeSpan.FromMinutes(30), "rankings_group");
        }
        
        var ranked = allWithRankings.FirstOrDefault(p => p.Id == id);
        if (ranked == null) return NotFound();
        
        return Ok(new PlayerRankResponse {
            GlobalRank = ranked.GlobalRank,
            PerformanceScore = ranked.PerformanceScore,
            SportStats = ranked.SportStats
        });
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
        var player = await _playerService.GetByIdAsync(id, cancellationToken);
        if (player != null)
        {
            var playerCacheKey = $"player_profile_{id}";
            await _redisService.RemoveDataAsync(playerCacheKey);

            if (player.UserId.HasValue)
            {
                var cacheKey = $"player_profile_{player.UserId.Value}";
                await _redisService.RemoveDataAsync(cacheKey);

                var userCacheKey = $"user_account_{player.UserId.Value}";
                await _redisService.RemoveDataAsync(userCacheKey);
            }
        }

        await _playerService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var player = await _playerService.GetByIdAsync(id, cancellationToken);
        if (player != null)
        {
            var playerCacheKey = $"player_profile_{id}";
            await _redisService.RemoveDataAsync(playerCacheKey);

            if (player.UserId.HasValue)
            {
                var cacheKey = $"player_profile_{player.UserId.Value}";
                await _redisService.RemoveDataAsync(cacheKey);

                var userCacheKey = $"user_account_{player.UserId.Value}";
                await _redisService.RemoveDataAsync(userCacheKey);
            }
        }

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
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetRankings([FromQuery] long? seasonId, CancellationToken cancellationToken)
    {
        try {
            var results = await _playerService.GetAllWithRankingsAsync(seasonId, cancellationToken);
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
        var userId = User.GetUserId();
        var matches = await _playerService.FindPotentialArchivalMatchesAsync(userId, firstName, lastName, group, cancellationToken);
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
        var requests = await _playerService.GetAllMergeRequestsAsync(cancellationToken);
        var req = requests.FirstOrDefault(r => r.Id == id);
        if (req != null)
        {
            var cacheKey = $"player_profile_{req.UserId}";
            await _redisService.RemoveDataAsync(cacheKey);

            var userCacheKey = $"user_account_{req.UserId}";
            await _redisService.RemoveDataAsync(userCacheKey);

            var targetPlayer = await _playerService.GetByIdAsync(req.TargetPlayerId, cancellationToken);
            if (targetPlayer != null)
            {
                var targetPlayerCacheKey = $"player_profile_{req.TargetPlayerId}";
                await _redisService.RemoveDataAsync(targetPlayerCacheKey);

                if (targetPlayer.UserId.HasValue)
                {
                    var targetUserCacheKey = $"player_profile_{targetPlayer.UserId.Value}";
                    await _redisService.RemoveDataAsync(targetUserCacheKey);

                    var targetUserAccountCacheKey = $"user_account_{targetPlayer.UserId.Value}";
                    await _redisService.RemoveDataAsync(targetUserAccountCacheKey);
                }
            }

            var requestingPlayer = await _playerService.GetByUserIdAsync(req.UserId, cancellationToken);
            if (requestingPlayer != null && requestingPlayer.Id.HasValue)
            {
                var requestingPlayerCacheKey = $"player_profile_{requestingPlayer.Id.Value}";
                await _redisService.RemoveDataAsync(requestingPlayerCacheKey);
            }
        }

        await _playerService.ApproveMergeRequestAsync(id, cancellationToken);
        await _redisService.RemoveGroupAsync("player_profiles_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("merge-requests/{id}/reject")]
    public async Task<IActionResult> RejectMergeRequest(long id, [FromQuery] string comment, CancellationToken cancellationToken)
    {
        await _playerService.RejectMergeRequestAsync(id, comment, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("merge-requests/all")]
    public async Task<ActionResult<IEnumerable<ProfileMergeRequestDto>>> GetAllMergeRequests(CancellationToken cancellationToken)
    {
        var requests = await _playerService.GetAllMergeRequestsAsync(cancellationToken);
        return Ok(requests);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("merge-manual")]
    public async Task<IActionResult> MergeManual([FromQuery] long sourcePlayerId, [FromQuery] long targetPlayerId, CancellationToken cancellationToken)
    {
        var sourcePlayer = await _playerService.GetByIdAsync(sourcePlayerId, cancellationToken);
        var targetPlayer = await _playerService.GetByIdAsync(targetPlayerId, cancellationToken);
        
        if (sourcePlayer != null)
        {
            var sourcePlayerCacheKey = $"player_profile_{sourcePlayerId}";
            await _redisService.RemoveDataAsync(sourcePlayerCacheKey);

            if (sourcePlayer.UserId.HasValue)
            {
                var cacheKey = $"player_profile_{sourcePlayer.UserId.Value}";
                await _redisService.RemoveDataAsync(cacheKey);

                var userCacheKey = $"user_account_{sourcePlayer.UserId.Value}";
                await _redisService.RemoveDataAsync(userCacheKey);
            }
        }

        if (targetPlayer != null)
        {
            var targetPlayerCacheKey = $"player_profile_{targetPlayerId}";
            await _redisService.RemoveDataAsync(targetPlayerCacheKey);

            if (targetPlayer.UserId.HasValue)
            {
                var cacheKey = $"player_profile_{targetPlayer.UserId.Value}";
                await _redisService.RemoveDataAsync(cacheKey);

                var userCacheKey = $"user_account_{targetPlayer.UserId.Value}";
                await _redisService.RemoveDataAsync(userCacheKey);
            }
        }

        await _playerService.MergePlayersManuallyAsync(sourcePlayerId, targetPlayerId, cancellationToken);
        await _redisService.RemoveGroupAsync("player_profiles_group");
        return Ok();
    }

    [Authorize]
    [HttpPost("merge-requests/ignore")]
    public async Task<IActionResult> IgnoreMergeRequest([FromQuery] long targetPlayerId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _playerService.IgnoreMergeRequestAsync(userId, targetPlayerId, cancellationToken);
        return Ok();
    }

    [HttpGet("head-to-head")]
    public async Task<ActionResult<HeadToHeadDto>> GetHeadToHead([FromQuery] long p1, [FromQuery] long p2, CancellationToken cancellationToken)
    {
        var result = await _playerService.GetHeadToHeadAsync(p1, p2, cancellationToken);
        return Ok(result);
    }
}

