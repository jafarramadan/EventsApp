using EventsApp.Models.Dto;

namespace EventsApp.Repository.Interfaces
{
    public interface IAccountx
    {
        public Task<AccountDto> Register(RegisterdAccountDto registerdAccountDto);
        public Task<AccountDto> AccountAuthentication(string username, string password);
        public Task LogOut(string username);
    }
}
