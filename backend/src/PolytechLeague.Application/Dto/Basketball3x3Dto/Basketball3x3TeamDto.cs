using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Application.Dto.Basketball3x3Dto;

public class Basketball3x3TeamRequest
{
    public required string Name { get; set; }
}

public class Basketball3x3TeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
}

public static class Basketball3x3TeamDtoExtensions
{
    public static Basketball3x3Team FromRequest(this Basketball3x3TeamRequest request)
    {
        return new Basketball3x3Team
        {
            Name = request.Name
        };
    }

    public static Basketball3x3TeamResponse ToResponse(this Basketball3x3Team entity)
    {
        return new Basketball3x3TeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList()
        };
    }
}
