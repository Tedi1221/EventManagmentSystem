using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.Models
{
    public class EventParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}