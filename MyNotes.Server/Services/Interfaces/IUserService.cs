using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.ViewModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(User user);
        Task<UserJwtModel> CreateOrFindUser(Payload payload);
        Task<UserViewModel> GetByEmailAsync(string email);
        Task<UserViewModel> GetUserById(int id);
        Task<UserViewModel> Login(LoginRequest model);
        Task<UserViewModel> Register(RegisterRequest model);
    }
}
