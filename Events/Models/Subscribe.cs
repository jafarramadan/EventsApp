using System.ComponentModel.DataAnnotations;

namespace EventsApp.Models
{
    public class Subscribe
    {
        public int SubscribeId { get; set; }
        [Required(ErrorMessage = "Participant Name is required")]
        public string ParticipantName { get; set; }
        [Required(ErrorMessage = "Participant Name is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
