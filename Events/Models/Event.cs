namespace EventsApp.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Capacity { get; set; }
        public ICollection<Subscribe> Subscribes { get; set; }
    }
}
