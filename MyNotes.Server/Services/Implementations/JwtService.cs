using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyNotes.Server.Common;
using MyNotes.Server.Controllers;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
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

        public string GenerateToken(string email, string username, string profileImageUrl)
        {
            var googleAuth = AppParameters.AppSettings.GoogleAuth;
            if (googleAuth == null)
            {
                throw new InvalidOperationException("GoogleAuth settings are not configured.");
            }

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(googleAuth.ClientSecret)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("username", username ?? ""),
                new Claim("avatar", profileImageUrl ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: googleAuth.Issuer,
                audience: googleAuth.ClientId,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Payload> ValidateGoogleRequest(GoogleLoginRequest request)
        {
            return await ValidateAsync(request.IdToken,
                new ValidationSettings
                {
                    Audience = new[] { AppParameters.AppSettings.GoogleAuth.ClientId }
                });
        }
    }

}
