namespace MyNotes.Server.Common
{
    public class AppSettings
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
