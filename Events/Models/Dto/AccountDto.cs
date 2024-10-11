namespace EventsApp.Models.Dto
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IList<string> Role { get; set; }
    }
}
