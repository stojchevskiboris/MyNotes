using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.ViewModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> CreateOrFindGoogleUser(Payload payload);
        Task<UserViewModel> GetByEmailAsync(string email);
        Task<UserViewModel> GetUserById(int id);
        Task<UserViewModel> Login(UserLoginModel model);
        Task<UserViewModel> Register(UserRegisterModel model);
        Task<UserViewModel> UpdateUser(UserViewModel model);
    }
}
