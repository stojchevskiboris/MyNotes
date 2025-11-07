using Google.Apis.Auth;
using MyNotes.Server.Common;
using MyNotes.Server.Common.Exceptions;
using MyNotes.Server.Common.Helpers;
using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
using MyNotes.Server.Services.Mappers;
using MyNotes.Server.Services.ViewModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MyNotes.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task CreateAsync(User user)
        {
            // Dummy implementation for demonstration purposes
            return Task.CompletedTask;
        }

        public async Task<UserJwtModel> CreateOrFindUser(Payload payload)
        {
            // Find or create user
            var user = await _userRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Username = payload.Name ?? payload.Email,
                    PasswordHash = null, // not needed for Google users
                    AuthProvider = AppParameters.AppSettings.GoogleAuth.AuthUri,
                    ProviderId = payload.Subject,
                    ProfileImageUrl = payload.Picture,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _userRepository.Create(user);
            }
            else
            {
                user.AuthProvider ??= AppParameters.AppSettings.GoogleAuth.AuthUri;
                user.ProviderId ??= payload.Subject;
                user.ProfileImageUrl ??= payload.Picture;
                user.LastLoginAt = DateTime.UtcNow;

                _userRepository.Update(user);
            }

            return user.MapToJwtModel();
        }

        public async Task<UserViewModel> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                return user.MapToViewModel();
            }

            throw new CustomException($"No user found with email: {email}");
        }

        public async Task<UserViewModel> GetUserById(int id)
        {
            var user = await GetUserDomainById(id);

            return user.MapToViewModel();
        }

        public async Task<UserViewModel> Login(LoginRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
                throw new ArgumentException("Invalid email or password.");

            if (user.IsGoogleUser)
                throw new ArgumentException("This account uses Google sign-in. Use the Google sign-in button.");

            var hashed = PasswordHelper.HashPassword(model.Password);
            if (!string.Equals(hashed, user.PasswordHash, StringComparison.Ordinal))
                throw new ArgumentException("Invalid email or password.");

            return user.MapToViewModel();
        }

        public async Task<UserViewModel> Register(RegisterRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Username))
            {
                throw new ArgumentException("Username, email and password are required.");
            }

            var existing = await _userRepository.GetByEmailAsync(model.Email);
            if (existing != null)
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            if (!PasswordHelper.CheckPasswordStrength(model.Password))
            {
                throw new ArgumentException("Password does not meet strength requirements.");
            }

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                AuthProvider = "Local",
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Create(user);

            return user.MapToViewModel();
        }

        private async Task<User> GetUserDomainById(int id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                throw new CustomException($"No user found with id: {id}");
            }

            return user;
        }
    }

}
