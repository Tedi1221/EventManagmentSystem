using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}