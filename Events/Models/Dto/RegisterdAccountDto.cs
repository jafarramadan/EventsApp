namespace EventsApp.Models.Dto
{
    public class RegisterdAccountDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IList<string> Role { get; set; }
    }
}
