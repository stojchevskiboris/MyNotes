using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;

namespace MyNotes.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;

        public UserService()
        {
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
    }

}
