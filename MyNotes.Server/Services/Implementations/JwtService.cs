using Microsoft.IdentityModel.Tokens;
using MyNotes.Server.Common;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
using MyNotes.Server.Services.Mappers;
using MyNotes.Server.Services.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public UserJwtModel GenerateTokenModel(UserViewModel userModel)
        {
            var googleAuth = AppParameters.AppSettings.GoogleAuth;
            var localAuth = AppParameters.AppSettings.LocalAuth;
            if (googleAuth == null || localAuth == null)
            {
                throw new InvalidOperationException("Auth settings are not configured.");
            }

            var isGoogleUser = userModel.IsGoogleUser;
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    isGoogleUser ? googleAuth.ClientSecret : localAuth.Secret
                )
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new Claim(ClaimTypes.Email, userModel.Email),
                new Claim("name", userModel.Name ?? ""),
                new Claim("avatar", userModel.ProfileImageUrl ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: isGoogleUser ? googleAuth.Issuer : localAuth.Issuer,
                audience: isGoogleUser ? googleAuth.ClientId : localAuth.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            var jwtSerializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            var resultModel = userModel.MapToUserJwtModel(jwtSerializedToken);

            return resultModel;
        }

        public async Task<Payload> ValidateGoogleRequest(GoogleUserLoginModel request)
        {
            return await ValidateAsync(request.IdToken,
                new ValidationSettings
                {
                    Audience = new[] { AppParameters.AppSettings.GoogleAuth.ClientId }
                });
        }
    }

}
