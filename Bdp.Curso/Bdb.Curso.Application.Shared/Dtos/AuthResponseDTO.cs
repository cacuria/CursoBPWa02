namespace Bdb.Curso.Application.Shared.Dtos
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}
