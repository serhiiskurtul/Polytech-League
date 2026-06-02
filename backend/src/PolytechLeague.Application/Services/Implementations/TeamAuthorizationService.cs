using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Utilities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations;

public class TeamAuthorizationService(ApplicationDbContext dbContext) : ITeamAuthorizationService
{
    public async Task ValidateFizorgAccessAsync(long userId, string teamName, long eventId, IEnumerable<long>? playerIdsToAdd, CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (player == null) throw new UnauthorizedAccessException("Ваш акаунт ще не прив'язано до профілю студента.");

        var fizorgGroup = GroupUtility.GetCurrentGroupName(player.SpecialtyCode, player.EnrollmentYear, player.GroupNumber);
        
        if (!string.Equals(teamName, fizorgGroup, StringComparison.OrdinalIgnoreCase))
        {
             throw new UnauthorizedAccessException($"Ви можете редагувати лише команду вашої групи ({fizorgGroup}).");
        }

        var tournament = await dbContext.Events.FindAsync(new object[] { eventId }, cancellationToken);
        if (tournament == null) throw new ArgumentException("Турнір не знайдено");

        var now = DateTime.UtcNow;
        if (now < tournament.RegistrationStart || now > tournament.RegistrationEnd)
        {
            throw new UnauthorizedAccessException("Реєстрація на турнір закрита.");
        }

        if (playerIdsToAdd != null && playerIdsToAdd.Any())
        {
            var requestedPlayers = await dbContext.Players.Where(p => playerIdsToAdd.Contains(p.Id)).ToListAsync(cancellationToken);
            foreach (var reqPlayer in requestedPlayers)
            {
                var reqGroup = GroupUtility.GetCurrentGroupName(reqPlayer.SpecialtyCode, reqPlayer.EnrollmentYear, reqPlayer.GroupNumber);
                if (reqGroup != fizorgGroup)
                {
                    throw new UnauthorizedAccessException($"Гравець {reqPlayer.LastName} не належить до вашої групи ({fizorgGroup}).");
                }
            }
        }
    }
}
