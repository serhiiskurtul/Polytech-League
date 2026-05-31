using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolytechLeague.Domain.Entities
{
    public class SiteVisit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime VisitDate { get; set; }

        public string? GuestId { get; set; } // UUID stored in local storage for guests

        public long? UserId { get; set; } // To identify registered users

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}
