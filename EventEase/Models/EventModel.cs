using System;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventModel : IValidatableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (Date < DateTime.Today)
                yield return new ValidationResult("Event date must be today or later.", new[] { nameof(Date) });
        }
    }
}
