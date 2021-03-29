namespace DummyWebApp.BLL.Options
{
    public class AccessTokenOptions
    {
        public string? Issuer { get; set; }

        public string? Audience { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int RefreshTokenLifetime { get; set; }

        public string? Key { get; set; }

        public bool ValidateActor { get; set; }

        public bool ValidateAudience { get; set; }

        public bool ValidateLifetime { get; set; }

        public bool ValidateIssuerSigningKey { get; set; }
    }
}