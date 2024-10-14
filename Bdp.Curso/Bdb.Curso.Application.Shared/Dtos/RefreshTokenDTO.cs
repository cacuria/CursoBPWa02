namespace Bdb.Curso.Application.Shared.Dtos
{
    public class RefreshTokenDto
    {
        public string? Token { get; set; }
        public DateTime Expires { get; set; }
    }


}
