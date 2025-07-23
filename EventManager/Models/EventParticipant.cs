namespace EventManagementSystem.Models
{
    // Join Table за връзка User <-> Event
    public class EventParticipant
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}