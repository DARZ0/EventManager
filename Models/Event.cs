using System.ComponentModel.DataAnnotations;

namespace EventEaseApp
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public string Location { get; set; } = "";
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false; // NEW
    }
}
