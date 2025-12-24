namespace MyNotes.Server.Domain.Models
{
    public class UserJwtModel
    {
        public int userId { get; set; } = 0;
        public string token { get; set; } = string.Empty;
        public UserViewModel? user { get; set; } = null;
    }
}
