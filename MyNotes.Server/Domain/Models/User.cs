namespace MyNotes.Server.Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
        public string? AuthProvider { get; set; }
        public string? ProviderId { get; set; }
        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        public bool IsGoogleUser { get; set; } = false;
    }
}
