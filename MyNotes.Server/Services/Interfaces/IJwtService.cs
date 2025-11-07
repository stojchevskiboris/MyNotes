using MyNotes.Server.Controllers;
using MyNotes.Server.Services.ViewModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string email, string username = "", string profileImageUrl = "");
        Task<Payload> ValidateGoogleRequest(GoogleLoginRequest request);
    }
}
