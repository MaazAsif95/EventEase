using System;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class RegistrationModel
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; }

        // Attendance tracking
        public bool Attended { get; set; }
        public DateTime? CheckedAt { get; set; }
        public string? CheckedBy { get; set; }
    }
}
