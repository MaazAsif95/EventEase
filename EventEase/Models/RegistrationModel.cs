using System;

namespace EventEase.Models
{
    public class RegistrationModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }

        // Attendance tracking
        public bool Attended { get; set; }
        public DateTime? CheckedAt { get; set; }
        public string? CheckedBy { get; set; }
    }
}
