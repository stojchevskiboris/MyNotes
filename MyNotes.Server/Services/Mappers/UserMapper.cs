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
                Name = user.Name,
                Email = user.Email,
                AuthProvider = user.AuthProvider,
                ProviderId = user.ProviderId,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsGoogleUser = user.IsGoogleUser,
                Theme = user.Theme,
            };

            return model;
        }

        public static UserJwtModel MapToUserJwtModel(this UserViewModel user, string jwtToken)
        {
            return new UserJwtModel
            {
                user = user,
                token = jwtToken,
                userId = user.Id,
            };
        }
    }
}
