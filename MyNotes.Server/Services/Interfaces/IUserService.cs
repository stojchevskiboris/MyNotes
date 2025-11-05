using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(User user);
        Task<User> GetByEmailAsync(string email);
    }
}
