namespace CampusLeague.Domain.Entities;

public class EventPhotos
{
    public long Id { get; set; }
    public required string Url { get; set; }
    public long EventId { get; set; }
    public Event? Event { get; set; }
}
