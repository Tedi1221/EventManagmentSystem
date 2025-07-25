using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Датата е задължителна.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Мястото е задължително.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Цената трябва да е положително число.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Максималният брой участници трябва да е поне 1.")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Категорията е задължителна.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        [Display(Name = "Снимка на събитието")]
        public IFormFile? ImageFile { get; set; }
        public string? ExistingImageUrl { get; set; }
    }
}
