namespace Bdb.Curso.Application.Shared.Dtos
{
    public class JwtSettingsValues
    {                                          
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiresInMinutes { get; set; }

        public string? PrivateKeyPath { get; set; }
        public string? PublicKeyPath { get; set; }
                                                                  
        public int RefreshTokenExpiresInDays { get; set; }
    }

}
