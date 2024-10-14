namespace Bdb.Curso.Application.Shared.Dtos
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public RefreshTokenDTO RefreshToken { get; set; }
    }
}
