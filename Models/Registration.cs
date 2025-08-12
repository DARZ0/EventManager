// File: Models/Registration.cs
using System.ComponentModel.DataAnnotations;

namespace EventEaseApp
{
    public class Registration
    {
        [Required]
        public string AttendeeName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        public Event? RegisteredEvent { get; set; }
        public bool IsAttended { get; set; } = false; // NEW
        public string? UserName { get; set; }
    }

}
