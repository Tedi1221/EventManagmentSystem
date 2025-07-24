// Път: Models/EventParticipant.cs
namespace EventManagementSystem.Models
{
    public class EventParticipant
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }
    }
}