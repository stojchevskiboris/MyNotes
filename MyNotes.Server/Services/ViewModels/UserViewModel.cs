namespace MyNotes.Server.Domain.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Authentication
        public string? AuthProvider { get; set; }
        public string? ProviderId { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Helpers
        public bool IsGoogleUser => AuthProvider?.Equals("Google", StringComparison.OrdinalIgnoreCase) == true;
    }
}
