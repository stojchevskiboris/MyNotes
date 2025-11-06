namespace MyNotes.Server.Common
{
    public class AppParameters
    {
        public static string ConnectionString = string.Empty;

        public static class AppSettings
        {
            public static string AesSecretKey { get; set; } = string.Empty;
            public static GoogleAuth GoogleAuth { get; set; } = new GoogleAuth();
        }
    }

    public class AppSettingsModel
    {
        public string AesSecretKey { get; set; } = string.Empty;
        public GoogleAuth GoogleAuth { get; set; } = new GoogleAuth();
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
}
