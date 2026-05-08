using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolytechLeague.Domain.Entities;

public class PushSubscription
{
    [Key]
    public int Id { get; set; }

    [Required]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [Required]
    public string Endpoint { get; set; }

    [Required]
    public string P256DH { get; set; }

    [Required]
    public string Auth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
