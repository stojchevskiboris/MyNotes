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

        public async Task<UserViewModel> CreateOrFindGoogleUser(Payload payload)
        {
            var user = await _userRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    PasswordHash = null,
                    AuthProvider = AppParameters.AppSettings.GoogleAuth.AuthUri,
                    ProviderId = payload.Subject,
                    ProfileImageUrl = payload.Picture,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    IsGoogleUser = true,
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

            return user.MapToViewModel();
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

        public async Task<UserViewModel> Login(UserLoginModel model)
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

            user.LastLoginAt = DateTime.Now;
            _userRepository.Update(user);

            return user.MapToViewModel();
        }

        public async Task<UserViewModel> Register(UserRegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new ArgumentException("Password is required.");

            if (model.Password != model.ConfirmPassword)
                throw new ArgumentException("Password and Confirm Password do not match.");

            if (!Validators.IsValidEmailAddress(model.Email))
                throw new ArgumentException("Please enter a valid email address.");

            var existing = await _userRepository.GetByEmailAsync(model.Email);
            if (existing != null)
                throw new ArgumentException("A user with this email already exists.");

            PasswordHelper.ValidatePasswordStrength(model.Password);

            var user = new User
            {
                Email = model.Email,
                Name = model.Name,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                AuthProvider = "Local",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsGoogleUser = false,
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
