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
    public EventCategory Category { get; set; } = EventCategory.Mixed;
    public string? Description { get; set; }
    public string? MainPhotoUrl { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; } = false;
    public bool IsPublic { get; set; } = true;
    public long? SeasonId { get; set; }
    public Season? Season { get; set; }

    public long? FirstPlaceId { get; set; }
    public Player? FirstPlace { get; set; }
    public long? SecondPlaceId { get; set; }
    public Player? SecondPlace { get; set; }
    public long? ThirdPlaceId { get; set; }
    public Player? ThirdPlace { get; set; }
}

public enum CompetitionLevel
{
    CollegeGroup,   // між групами коледжу
    Regional,       // обласні змагання
    National        // всеукраїнські
}

public enum EventCategory
{
    Men,
    Women,
    Mixed
}


