using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ITeamAuthorizationService
{
    Task ValidateFizorgAccessAsync(long userId, string teamName, long eventId, IEnumerable<long>? playerIdsToAdd, CancellationToken cancellationToken);
}
