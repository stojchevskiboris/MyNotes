using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.ViewModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Interfaces
{
    public interface IJwtService
    {
        UserJwtModel GenerateTokenModel(UserViewModel userModel);
        Task<Payload> ValidateGoogleRequest(GoogleUserLoginModel request);
    }
}
