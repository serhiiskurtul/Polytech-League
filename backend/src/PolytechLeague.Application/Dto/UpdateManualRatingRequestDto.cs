namespace PolytechLeague.Application.Dto;

public class UpdateManualRatingRequestDto
{
    public required List<ManualRatingEntryDto> Entries { get; set; }
    public List<string>? ActiveSports { get; set; }
}
