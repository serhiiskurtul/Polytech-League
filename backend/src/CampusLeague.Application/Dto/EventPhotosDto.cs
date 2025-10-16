using CampusLeague.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace CampusLeague.Application.Dto;

public class EventPhotosRequest
{
    public required List<IFormFile> Photos { get; set; }
    public long EventId { get; set; }
}

public class EventPhotosResponse
{
    public long Id { get; set; } // можна вважати EventId
    public long EventId { get; set; }
    public required List<EventPhotoDto> Photos { get; set; }
}

public class EventPhotoDto
{
    public long Id { get; set; }
    public required string Url { get; set; }
}

public static class EventPhotosDtoExtension
{
    public static EventPhotoDto ToDto(this EventPhotos photo)
    {
        return new EventPhotoDto
        {
            Id = photo.Id,
            Url = photo.Url
        };
    }
    
    public static EventPhotosResponse ToResponse(this IEnumerable<EventPhotos> eventPhotosList, long eventId)
    {
        return new EventPhotosResponse
        {
            Id = eventId,
            EventId = eventId,
            Photos = eventPhotosList.Select(p => new EventPhotoDto
            {
                Id = p.Id,
                Url = p.Url
            }).ToList()
        };
    }
}
