using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace EventManagementSystem.Models
{
    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Име на събитието")]
        [Required(ErrorMessage = "Моля, въведете име на събитието.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Моля, въведете описание.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Дата и час")]
        [Required(ErrorMessage = "Моля, въведете дата и час.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Място")]
        [Required(ErrorMessage = "Моля, въведете място.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Цена")]
        [Range(0, double.MaxValue, ErrorMessage = "Цената трябва да е положително число.")]
        public decimal Price { get; set; }

        [Display(Name = "Макс. участници")]
        [Range(1, int.MaxValue, ErrorMessage = "Броят участници трябва да е поне 1.")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Моля, изберете категория.")]
        public int CategoryId { get; set; }

        [Display(Name = "Снимка на събитието")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}