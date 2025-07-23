using Microsoft.AspNetCore.Identity;

namespace EventManagementSystem.Models
{
    public class User : IdentityUser
    {
        public ICollection<EventParticipant> AttendingEvents { get; set; } = new List<EventParticipant>();
    }
}
