using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.DataLayer.Models
{
    public sealed class ReferralLog
    {
        public int Id { get; init; }

        public int AmbassadorId { get; init; }

        [ForeignKey(nameof(AmbassadorId))]
        public User? Ambassador { get; set; }

        public int ReferredUserId { get; init; }

        [ForeignKey(nameof(ReferredUserId))]
        public User? ReferredUser { get; set; }

        public int EventId { get; init; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
