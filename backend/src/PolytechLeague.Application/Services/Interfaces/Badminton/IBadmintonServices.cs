using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Application.Services.Interfaces.Badminton;

public interface IBadmintonEventService : IAbstractEntityService<BadmintonEvent> { }
public interface IBadmintonMatchService : IAbstractEntityService<BadmintonMatch> { }
public interface IBadmintonPlayerService : IAbstractEntityService<BadmintonPlayer> { }
public interface IBadmintonParticipantService : IAbstractEntityService<BadmintonEventParticipant> { }
