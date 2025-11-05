namespace MyNotes.Server.Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        // Basic Identity
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Authentication
        public string? PasswordHash { get; set; }     // null for OAuth users
        public string? AuthProvider { get; set; }     // e.g. "Local", "Google"
        public string? ProviderId { get; set; }       // Google user ID (sub)
        public string? ProfileImageUrl { get; set; }  // Google avatar or custom

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Navigation Properties
        //public ICollection<Note> Notes { get; set; } = new List<Note>();

        // Helpers
        public bool IsGoogleUser => AuthProvider?.Equals("Google", StringComparison.OrdinalIgnoreCase) == true;
    }
}
