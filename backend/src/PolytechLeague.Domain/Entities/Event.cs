namespace PolytechLeague.Domain.Entities;

public abstract class Event
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public ICollection<EventPhotos>? Photos { get; set; }
    public required ICollection<long> RefereeIds { get; set; }
    public required CompetitionLevel Level { get; set; }
    public required string Description { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Schedule { get; set; }
}

public enum CompetitionLevel
{
    CollegeGroup,   // між групами коледжу
    Regional,       // обласні змагання
    National        // всеукраїнські
}


