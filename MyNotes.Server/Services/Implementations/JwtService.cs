using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyNotes.Server.Common;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyNotes.Server.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;

        public JwtService(IConfiguration config, IOptions<AppSettings> appSettings)
        {
            _config = config;
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(string email)
        {
            var googleAuth = _appSettings.GoogleAuth;
            if (googleAuth == null)
            {
                throw new InvalidOperationException("GoogleAuth settings are not configured.");
            }

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(googleAuth.ClientSecret)
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email)
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
    }

}
