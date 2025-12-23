namespace MyNotes.Server.Common
{
    public class AppParameters
    {
        public static string ConnectionString = string.Empty;

        public static class AppSettings
        {
            public static string AesSecretKey { get; set; } = string.Empty;
            public static GoogleAuth GoogleAuth { get; set; } = null!;
            public static JwtSettings Jwt { get; set; } = null!;
        }
    }

    public class AppSettingsModel
    {
        public string AesSecretKey { get; set; } = string.Empty;
        public GoogleAuth GoogleAuth { get; set; } = null!;
        public JwtSettings Jwt { get; set; } = null!;

    }

    public class GoogleAuth
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
        public string AuthUri { get; set; } = null!;
        public string TokenUri { get; set; } = null!;
        public string AuthProviderCertUrl { get; set; } = null!;
        public string Issuer { get; set; } = null!;
    }

    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpireMinutes { get; set; } = 0;
    }
}
