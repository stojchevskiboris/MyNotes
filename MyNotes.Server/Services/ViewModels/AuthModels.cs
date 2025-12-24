namespace MyNotes.Server.Services.ViewModels
{
    public class GoogleUserLoginModel
    {
        public string IdToken { get; set; } = "";
    }

    public class UserLoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UserRegisterModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
