using System.Text.RegularExpressions;

namespace MyNotes.Server.Services
{
    public static class Validators
    {
        public static bool IsValidEmailAddress(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                return false;

            return Regex.IsMatch(emailAddress, @"^[\w\-\.]+@([\w\-]+\.)+[\w\-]{2,4}$");
        }
    }
}
