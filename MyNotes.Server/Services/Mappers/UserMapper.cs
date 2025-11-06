using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Services.Mappers
{
    public static class UserMapper
    {
        public static UserViewModel MapToViewModel(this User user)
        {
            if (user == null)
                return null;

            var model = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AuthProvider = user.AuthProvider,
                ProviderId = user.ProviderId,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return model;
        }
    }
}
