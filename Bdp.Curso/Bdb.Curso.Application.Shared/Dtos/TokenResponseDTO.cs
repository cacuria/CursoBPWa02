namespace Bdb.Curso.Application.Shared.Dtos
{
    public class TokenResponseDto
    {
        public string? AccessToken { get; set; }
        public RefreshTokenDto? RefreshToken { get; set; } = new RefreshTokenDto();
    }
}
