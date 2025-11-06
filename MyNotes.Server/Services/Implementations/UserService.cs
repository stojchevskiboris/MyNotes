using MyNotes.Server.Common.Exceptions;
using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
using MyNotes.Server.Services.Mappers;

namespace MyNotes.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
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

        public Task<User> GetByEmailAsync(string email)
        {
            // Dummy implementation for demonstration purposes
            var user = new User
            {
                Id = 1,
                Email = email,
                Username = "DemoUser",
                CreatedAt = DateTime.UtcNow
            };
            return Task.FromResult(user);
        }

        public async Task<UserViewModel> GetUserById(int id)
        {
            var user = await GetUserDomainById(id);

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
